using System.Text;

namespace Unator.Email.Senders;

public class Resend : UEmailSender
{
    private const string url = "https://api.resend.com/emails";
    private readonly string token;

    private readonly long monthLimit;
    private readonly int dayLimit;
    private int dayUsed = 0;
    private long monthUsed = 0;

    /// <summary>
    /// Date of last month limit reset. We store it in long to use Interlocked.
    /// Represent DateTime Ticks.
    /// </summary>
    private long lastMonthReset;

    public Resend(string token, DateTime lastMonthReset, long monthLimit, int dayLimit)
    {
        this.token = token;
        this.lastMonthReset = lastMonthReset.Ticks;
        this.dayLimit = dayLimit;
        this.monthLimit = monthLimit;
    }

    public long GetMonthLimit() => monthLimit;

    /// <summary>
    /// Ensure that limits and it right state and check if limit allow to send new email.
    /// </summary>
    public bool IsLimitAllow()
    {
        DateTime now = DateTime.Now;

        // day limit reset
        if (now.TimeOfDay == TimeSpan.Zero) Interlocked.Exchange(ref dayUsed, 0);

        // month limit reset
        var daysBetween = (now.Ticks - lastMonthReset) / TimeSpan.TicksPerDay;
        if (daysBetween > 0 && daysBetween % 30 == 0)
        {
            Interlocked.Exchange(ref monthUsed, 0);
            Interlocked.Exchange(ref lastMonthReset, now.Ticks);
        }

        return dayUsed < dayLimit && monthUsed < monthLimit;
    }

    public async Task<Exception?> SendEmail(string from, string to, string subject, string html)
    {
        try
        {
            using HttpClient client = new();

            string jsonBody = $@"
            {{
                ""from"": ""{from}"",
                ""to"": ""{to}"",
                ""subject"": ""{subject}"",
                ""html"": ""{html}""
            }}";

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

            HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                Interlocked.Increment(ref monthUsed);
                Interlocked.Increment(ref dayUsed);
            }
            else
            {
                return new SenderServerFailException();
                // process different errors

                Console.WriteLine("failed");
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}