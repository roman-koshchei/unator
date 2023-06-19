using System.Text;

namespace Unator.Email.Senders;

/// <summary>
/// Brevo email sender. At the current moment Brevo doesn't have month limit.
/// </summary>
public class Brevo : UEmailSender
{
    private const string url = "https://api.brevo.com/v3/smtp/email";
    private readonly string token;

    private readonly int dayLimit;
    private int dayUsed = 0;

    public Brevo(string token, int dayLimit)
    {
        this.token = token;
        this.dayLimit = dayLimit;
    }

    public long GetMonthLimit() => dayLimit * 30;

    public bool IsLimitAllow()
    {
        // day limit reset
        if (DateTime.Now.TimeOfDay == TimeSpan.Zero) Interlocked.Exchange(ref dayUsed, 0);

        return dayUsed < dayLimit;
    }

    public async Task<Exception?> SendEmail(string from, string to, string subject, string html)
    {
        try
        {
            using HttpClient client = new();

            string jsonBody = $@"
            {{
                ""sender"": {{
                    ""email"":""{from}""
                }},
                ""to"":[
                    {{
                        ""email"":""{to}""
                    }}
                ],
                ""subject"":""{subject}"",
                ""htmlContent"":""{html}""
            }}";

            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("api-key", token);

            HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                Interlocked.Increment(ref dayUsed);
            }
            else
            {
                // process different errors
                Console.WriteLine("failed");
                return new Exception(response.StatusCode.ToString());
            }

            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}