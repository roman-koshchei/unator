using System.Text;

namespace Unator.Email.Senders;

public class Mailjet : UEmailSender
{
    private const string url = "https://api.mailjet.com/v3/send";
    private string credentials;

    private readonly long monthLimit;
    private readonly long dayLimit;

    public Mailjet(string key, string secret, long monthLimit, int dayLimit)
    {
        credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{key}:{secret}"));
        this.monthLimit = monthLimit;
        this.dayLimit = dayLimit;
    }

    public long GetMonthLimit() => monthLimit;

    public bool IsLimitAllow()
    {
        throw new NotImplementedException();
    }

    public async Task<Exception?> SendEmail(string from, string to, string subject, string html)
    {
        try
        {
            using HttpClient client = new();

            string jsonBody = $@"
            {{
                ""FromEmail"": ""{from}"",
                ""Recipients"": [
                    {{
                        ""Email"": ""{to}""
                    }}
                ],
                ""Subject"": ""{subject}"",
                ""Html-part"": ""{html}""
            }}";

            client.DefaultRequestHeaders.Add("Authorization", $"Basic {credentials}");

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