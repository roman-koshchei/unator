using System.Net.Http.Headers;
using System.Text;

namespace Unator.Email;

/// <summary>
/// Care only about sending email. Limits are controlled by ULimiter.
/// </summary>
public interface UEmailSender
{
    // somehow we should determine and limit possible exceptions returned from this method
    /// <summary>
    /// Send email.
    /// </summary>
    /// <param name="from">Email adress from what you send.</param>
    /// <param name="to">Email adress of destination.</param>
    /// <param name="subject">Subject of email.</param>
    /// <param name="html">Html content.</param>
    /// <returns>null if email is sended successfully and Exception if not.</returns>
    public Task<Exception?> SendOne(string from, string to, string subject, string html);

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