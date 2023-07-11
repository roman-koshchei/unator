using System.Net.Http.Headers;
using System.Text;

namespace Unator.Email;

public enum EmailStatus
{
    Success,
    LimitReached, // confusing, becasue don't know which limit
    Failed
}

/// <summary>
/// Care only about sending email. Limits are controlled by ULimiter.
/// </summary>
public interface UEmailSender
{
    // somehow we should determine and limit possible exceptions returned from this method
    /// <summary>
    /// Send email.
    /// </summary>
    /// <param name="fromEmail">Email adress from what you send.</param>
    /// <param name="fromName">Name of a sender.</param>
    /// <param name="to">List of email adresses of destination.</param>
    /// <param name="subject">Subject of email.</param>
    /// <param name="text">Text content.</param>
    /// <param name="html">Html content.</param>
    /// <returns>null if email is sended successfully and Exception if not.</returns>
    public Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html);

    protected static HttpClient JsonHttpClient(Action<HttpRequestHeaders> setHeaders)
    {
        HttpClient client = new();

        client.DefaultRequestHeaders.Add("Accept", "application/json");
        setHeaders(client.DefaultRequestHeaders);

        return client;
    }

    protected static async Task<HttpResponseMessage> JsonPost(HttpClient client, string url, string body)
    {
        return await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
    }
}

public class EmailService
{
    public UEmailSender Sender { get; }
    public ILimiter[] Limiters { get; }

    public EmailService(UEmailSender sender, params ILimiter[] limiters)
    {
        Sender = sender;
        Limiters = limiters;
    }
}