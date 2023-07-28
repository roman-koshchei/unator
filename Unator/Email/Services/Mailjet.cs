using System.Net.Http.Headers;
using System.Text;

namespace Unator.Email.Services;

public class Mailjet : UEmailSender
{
    private const string url = "https://api.mailjet.com/v3.1/send";
    private readonly HttpClient httpClient;

    public Mailjet(string key, string secret)
    {
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{key}:{secret}"));
        httpClient = Http.JsonClient(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        });
    }

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html)
    {
        string jsonBody = @$"{{
            ""Messages"":[{{
                ""From"":{{""Email"":""{fromEmail}""}},
                ""HTMLPart"":""{html}"",
                ""Subject"":""{subject}"",
                ""TextPart"":""{html}"",
                ""To"":[{{""Email"":""{to}""}}]
            }}]
        }}".Compact();

        var response = await Http.JsonPost(httpClient, url, jsonBody);
        if (response == null) return EmailStatus.Failed;

        if (response.IsSuccessStatusCode) return EmailStatus.Success;
        return EmailStatus.Failed;
    }
}