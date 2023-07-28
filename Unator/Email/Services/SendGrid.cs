using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Unator.Email.Senders;

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

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> toEmails, string subject, string text, string html)
    {
        try
        {
            string jsonBody = UEmailSender.Compact(@$"{{
                ""personalizations"":[{{
                    ""to"":[{string.Join(",", toEmails.Select(x => $@"{{""email"":""{x}""}}"))}]
                }}],
                ""from"":{{
                    ""email"":""{fromEmail}"",
                    ""name"":""{fromName}""
                }},
                ""reply_to"":{{
                    ""email"":""{fromEmail}"",
                    ""name"":""{fromName}""
                }},
                ""subject"":""{subject}"",
                ""content"": [
                    {{
                        ""type"": ""text/plain"",
                        ""value"": ""{text}""
                    }},
                    {{
                        ""type"": ""text/html"",
                        ""value"": ""{html}""
                    }}
                ]
            }}");

            var response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            //string responseBody = await response.Content.ReadAsStringAsync();
            //Console.WriteLine(responseBody);

            if (response.IsSuccessStatusCode) return EmailStatus.Success;
            if (response.StatusCode == HttpStatusCode.TooManyRequests) return EmailStatus.LimitReached;
            return EmailStatus.Failed;
        }
        catch
        {
            return EmailStatus.Failed;
        }
    }
}