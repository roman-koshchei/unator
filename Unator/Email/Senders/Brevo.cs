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

    public async Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        try
        {
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

            HttpResponseMessage response = await UEmailSender.JsonPost(httpClient, url, jsonBody);

            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            if (response.IsSuccessStatusCode)
            {
                return null;
            }
            // process different errors
            Console.WriteLine("failed");
            return new Exception(response.StatusCode.ToString());
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}