using System.Net.Http;
using System.Net.Http.Headers;

namespace Unator.Email.Senders;

/*
public class SendGrid : UEmailSender
{
    private const string url = "https://api.sendgrid.com/v3/mail/send";
    private readonly HttpClient httpClient;

    public SendGrid(string key)
    {
        httpClient = UEmailSender.JsonHttpClient(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        });
    }

    public async Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        try
        {
            string jsonBody = @$"{{""personalizations"":[{{""to"":[{{""email"":""{to}"",""name"":""{to}""}}],""subject"":""{subject}""}}],""content"": [{{""type"": ""text/plain"", ""value"": ""{html}""}}],""from"":{{""email"":""{from}"",""name"":""{from}""}},""reply_to"":{{""email"":""{from}"",""name"":""{from}""}}}}";

            var response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

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
*/