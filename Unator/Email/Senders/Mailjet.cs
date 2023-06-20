using System.Net.Http.Headers;
using System.Text;

namespace Unator.Email.Senders;

public class Mailjet : UEmailSender
{
    private const string url = "https://api.mailjet.com/v3.1/send";
    private readonly HttpClient httpClient;

    public Mailjet(string key, string secret)
    {
        var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{key}:{secret}"));
        httpClient = UEmailSender.JsonHttpClient(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue("Basic", credentials);
        });
    }

    public async Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        try
        {
            string jsonBody = @$"{{""Messages"":[{{""From"":{{""Email"":""{from}""}},""HTMLPart"":""{html}"",""Subject"":""{subject}"",""TextPart"":""{html}"",""To"":[{{""Email"":""{to}""}}]}}]}}";

            HttpResponseMessage response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            if (response.IsSuccessStatusCode)
            {
                return null;
            }

            return new SenderServerFailException();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}