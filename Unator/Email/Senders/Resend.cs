using System.Text;

namespace Unator.Email.Senders;

public class Resend : UEmailSender
{
    private const string url = "https://api.resend.com/emails";

    private readonly HttpClient httpClient;

    public Resend(string token)
    {
        httpClient = UEmailSender.JsonHttpClient(headers =>
        {
            headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        });
    }

    public async Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        try
        {
            string jsonBody = $@"
            {{
                ""from"": ""{from}"",
                ""to"": ""{to}"",
                ""subject"": ""{subject}"",
                ""html"": ""{html}""
            }}";

            HttpResponseMessage response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            if (response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
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