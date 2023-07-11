using System.Linq;

namespace Unator.Email.Senders;

/// <summary>
/// Brevo email sender. At the current moment Brevo doesn't have month limit.
/// </summary>
public class Brevo : UEmailSender
{
    private const string url = "https://api.brevo.com/v3/smtp/email";
    private readonly HttpClient httpClient;

    public Brevo(string token)
    {
        httpClient = UEmailSender.JsonHttpClient(headers =>
        {
            headers.Add("api-key", token);
        });
    }

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html)
    {
        try
        {
            string jsonBody = $@"
            {{
                ""sender"": {{
                    ""email"":""{fromEmail}""
                }},
                ""to"":[{string.Join(",", to.Select(x => $@"{{""email"":""{x}""}}"))}],
                ""subject"":""{subject}"",
                ""htmlContent"":""{html}""
            }}";

            HttpResponseMessage response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            if (response.IsSuccessStatusCode) return EmailStatus.Success;
            return EmailStatus.Failed;
        }
        catch
        {
            return EmailStatus.Failed;
        }
    }
}