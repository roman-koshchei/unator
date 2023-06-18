using System.Text;

namespace Unator.Email.Senders;

public class Resend : UEmailSender
{
    private const string url = "https://api.resend.com/emails";
    private readonly string token;

    public Resend(string token)
    {
        this.token = token;
    }

    public bool IsLimitAllow()
    {
        throw new NotImplementedException();
    }

    public async Task<Exception?> SendEmails(string from, string to, string subject, string html)
    {
        try
        {
            using HttpClient client = new();

            string jsonBody = $@"
            {{
                ""from"": ""{from}"",
                ""to"": ""{to}"",
                ""subject"": ""{subject}"",
                ""html"": ""{html}"",
            }}";

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            HttpResponseMessage response = await client.PostAsync(url, new StringContent(jsonBody, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
            }
            else
            {
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