using System.Net;

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

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html)
    {
        try
        {
            string jsonBody = $@"
            {{
                ""from"": ""{fromName} <{fromEmail}>"",
                ""to"": [{string.Join(",", to.Select(x => $@"""{x}"""))}],
                ""subject"": ""{subject}"",
                ""text"":""{text}"",
                ""html"": ""{html}""
            }}";

            HttpResponseMessage response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            if (response.IsSuccessStatusCode)
            {
                // during testing

                return EmailStatus.Success;
            }

            if (response.StatusCode == HttpStatusCode.TooManyRequests) return EmailStatus.LimitReached;

            return EmailStatus.Failed;
        }
        catch
        {
            return EmailStatus.Failed;
        }
    }
}