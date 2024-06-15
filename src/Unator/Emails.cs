using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Unator;

/*
    Services I am looking at now:
    |                                                       | Free limit               |
    | ----------------------------------------------------- | ------------------------ |
  + | [Resend] (https://resend.com/)                        | 3000/month (max 100/day) |
  + | [Brevo] (https://www.brevo.com/)                      | 300/day                  |
    | [SendGrid] (https://sendgrid.com/)                    | 100/day                  |
    | [Mailchimp] (https://mailchimp.com/)                  | 1000/month (max 500/day) |
 +- | [Mailjet] (https://www.mailjet.com/)                  | 6000/month (max 200/day) |
    | [Postmark] (https://postmarkapp.com/)                 | 100/month                |
    | [Elastic email] (https://elasticemail.com/)           | 100/day                  |
    | [Amazon SES] (https://aws.amazon.com/ru/ses/pricing/) | 62000/month ?            |
*/

public static class EmailExample
{
    public static async Task Run()
    {
        var email = new EmailSwitch(
            new EmailService(new Resend("resend-api-token"), new DayLimiter(100)),
            new EmailService(new Brevo("brevo-api-token"), new DayLimiter(300)),
            new EmailService(new SendGrid("send-grid-api-token"), new DayLimiter(100))
        );

        var sent = await email.Send("romankoshchei@gmail.com", "Roman Koshchei",
            to: ["roman@flurium.com"],
            subject: "I own Flurium",
            text: "Hi me from flurium account!",
            html: "<h1>Hi me from flurium account!</h1>"
        );

        Console.WriteLine(sent switch
        {
            EmailStatus.Success => "Success",
            EmailStatus.LimitReached => "Limits are reached",
            _ => "Can't send an email"
        });
    }
}

public interface ILimiter
{
    /// <summary>
    /// Ensure that limits and it right state and check if limit allow to send new email.
    /// </summary>
    /// <returns>true if limit allows, false otherwise</returns>
    bool IsLimitAllow();

    /// <summary>
    /// Increment limiter. Is called if email is sent successfully.
    /// </summary>
    void IncrementLimiter();
}

public class MonthLimiter : ILimiter
{
    private readonly long monthLimit;
    private long monthUsed = 0;

    /// <summary>
    /// Date of last month limit reset. We store it in long to use Interlocked.
    /// Represent DateTime Ticks.
    /// </summary>
    private long lastMonthReset;

    public MonthLimiter(DateTime lastMonthReset, long monthLimit)
    {
        this.monthLimit = monthLimit;
        this.lastMonthReset = lastMonthReset.Ticks;
    }

    public void IncrementLimiter() => Interlocked.Increment(ref monthUsed);

    public bool IsLimitAllow()
    {
        DateTime now = DateTime.Now;

        // because it's long fractional part is cut
        // so we have 0, 1, 2, ...
        long monthsBetween = (now.Ticks - lastMonthReset) / (TimeSpan.TicksPerDay * 30);

        // 1, 2, 3
        if (monthsBetween > 0)
        {
            long ticksToAdd = monthsBetween * 30 * TimeSpan.TicksPerDay;
            long newResetDate = lastMonthReset + ticksToAdd;

            Interlocked.Exchange(ref lastMonthReset, newResetDate);
            Interlocked.Exchange(ref monthUsed, 0);
        }

        return monthUsed < monthLimit;
    }
}

public class DayLimiter : ILimiter
{
    private readonly int dayLimit;
    private int dayUsed = 0;

    private long lastReset;

    public DayLimiter(int dayLimit)
    {
        this.dayLimit = dayLimit;
        lastReset = DateTime.Today.Ticks;
    }

    public void IncrementLimiter() => Interlocked.Increment(ref dayUsed);

    public bool IsLimitAllow()
    {
        DateTime now = DateTime.Now;

        if (now.Ticks - lastReset > TimeSpan.TicksPerDay)
        {
            Interlocked.Exchange(ref dayUsed, 0);
            Interlocked.Exchange(ref lastReset, now.Date.Ticks);
        }

        return dayUsed < dayLimit;
    }
}

public enum EmailStatus
{
    Success,
    LimitReached, // confusing, becasue don't know which limit
    Failed
}

/// <summary>Care only about sending email. Limits are controlled by ILimiter.</summary>
public interface IEmailSender
{
    /// <param name="fromEmail">Email adress from what you send.</param>
    /// <param name="fromName">Name of a sender.</param>
    /// <param name="to">List of email adresses of destination.</param>
    /// <param name="subject">Subject of email.</param>
    /// <param name="text">Text content.</param>
    /// <param name="html">Html content.</param>
    public Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html);
}

public class EmailService
{
    public IEmailSender Sender { get; }
    public ILimiter[] Limiters { get; }

    public EmailService(IEmailSender sender, params ILimiter[] limiters)
    {
        Sender = sender;
        Limiters = limiters;
    }
}

/// <summary>Manage several email senders.</summary>
public class EmailSwitch : IEmailSender
{
    private readonly IImmutableList<EmailService> services;

    public EmailSwitch(EmailService service, params EmailService[] services)
    {
        var list = new List<EmailService>(services.Length + 1) { service };
        list.AddRange(services);
        this.services = list.ToImmutableList();
    }

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html)
    {
        try
        {
            bool allLimitsReached = true;

            for (int i = 0; i < services.Count; ++i)
            {
                var service = services[i];

                if (service.Limiters.All(l => l.IsLimitAllow()))
                {
                    var status = await service.Sender.Send(fromEmail, fromName, to, subject, text, html);

                    if (status == EmailStatus.Success)
                    {
                        foreach (var limiter in service.Limiters) limiter.IncrementLimiter();
                        return EmailStatus.Success;
                    }

                    if (status == EmailStatus.Failed) allLimitsReached = false;
                }
            }

            return allLimitsReached ? EmailStatus.LimitReached : EmailStatus.Failed;
        }
        catch
        {
            return EmailStatus.Failed;
        }
    }
}

// RESEND

/// <summary>
/// Send emails with Resend service: <see href="https://resend.com">resend.com</see>
/// </summary>
public class Resend : IEmailSender
{
    private const string url = "https://api.resend.com/emails";

    private readonly HttpClient httpClient;

    public Resend(string token)
    {
        httpClient = Http.JsonClient(headers =>
        {
            headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        });
    }

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> toEmails, string subject, string text, string html)
    {
        string jsonBody = $@"
        {{
            ""from"":""{fromName} <{fromEmail}>"",
            ""to"":[{string.Join(",", toEmails.Select(x => $@"""{x}"""))}],
            ""subject"":""{subject}"",
            ""text"":""{text}"",
            ""html"":""{html}""
        }}".Compact();

        var response = await Http.JsonPost(httpClient, url, jsonBody);
        if (response == null) return EmailStatus.Failed;

        if (response.IsSuccessStatusCode) return EmailStatus.Success;
        if (response.StatusCode == HttpStatusCode.TooManyRequests) return EmailStatus.LimitReached;
        return EmailStatus.Failed;
    }
}

// MAILJET

public class Mailjet : IEmailSender
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

// BREVO
/// <summary>
/// Send emails with Brevo service: <see href="https://www.brevo.com">brevo.com</see>
/// </summary>
public class Brevo : IEmailSender
{
    private const string url = "https://api.brevo.com/v3/smtp/email";
    private readonly HttpClient httpClient;

    public Brevo(string token)
    {
        httpClient = Http.JsonClient(headers => headers.Add("api-key", token));
    }

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> to, string subject, string text, string html)
    {
        string jsonBody = $@"
        {{
            ""sender"":{{""name"":""{fromName}"",""email"":""{fromEmail}""}},
            ""to"":[{string.Join(",", to.Select(x => $@"{{""email"":""{x}""}}"))}],
            ""subject"":""{subject}"",
            ""htmlContent"":""{html}"",
            ""textContent"":""{text}""
        }}".Compact();

        var response = await Http.JsonPost(httpClient, url, jsonBody);
        if (response == null) return EmailStatus.Failed;

        if (response.IsSuccessStatusCode) return EmailStatus.Success;
        return EmailStatus.Failed;
    }
}

// SENDGRID

/// <summary>
/// Send emails with SendGrid service: <see href="https://sendgrid.com/">sendgrid.com</see>
/// </summary>
public class SendGrid : IEmailSender
{
    private const string url = "https://api.sendgrid.com/v3/mail/send";
    private readonly HttpClient httpClient;

    public SendGrid(string key)
    {
        httpClient = Http.JsonClient(headers =>
        {
            headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        });
    }

    public async Task<EmailStatus> Send(string fromEmail, string fromName, List<string> toEmails, string subject, string text, string html)
    {
        string jsonBody = @$"{{
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
        }}".Compact();

        var response = await Http.JsonPost(httpClient, url, jsonBody);
        if (response == null) return EmailStatus.Failed;

        if (response.IsSuccessStatusCode) return EmailStatus.Success;
        if (response.StatusCode == HttpStatusCode.TooManyRequests) return EmailStatus.LimitReached;
        return EmailStatus.Failed;
    }
}

/// <summary>Helper to work with Http.</summary>
public static class Http
{
    /// <summary>Create HttpClient to work with json.</summary>
    /// <param name="setHeaders">Action to set additional headers to client.</param>
    public static HttpClient JsonClient(Action<HttpRequestHeaders> setHeaders)
    {
        HttpClient client = new();

        client.DefaultRequestHeaders.Add("Accept", "application/json");
        setHeaders(client.DefaultRequestHeaders);

        return client;
    }

    /// <summary>Read json from <paramref name="response"/>.</summary>
    /// <returns>Null if error otherwise data of type <typeparamref name="T"/></returns>
    public static async Task<T?> JsonResponse<T>(HttpResponseMessage response) where T : class
    {
        try
        {
            var jsonStream = await response.Content.ReadAsStreamAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = await JsonSerializer.DeserializeAsync<T>(jsonStream, options);
            return data;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Send POST request to specified url with json body.
    /// </summary>
    /// <param name="body">JSON string</param>
    /// <returns>Null if exception appeared, response otherwise.</returns>
    public static async Task<HttpResponseMessage?> JsonPost(HttpClient client, string url, string body)
    {
        try
        {
            return await client.PostAsync(url, new StringContent(body, Encoding.UTF8, "application/json"));
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Check if status code is successful.</summary>
    /// <returns>True if success, false if not.</returns>
    public static bool IsSuccessful(int statusCode)
    {
        return (statusCode >= 200) && (statusCode <= 299);
    }

    /// <summary>Check if status code is successful.</summary>
    /// <returns>True if success, false if not.</returns>
    public static bool IsSuccessful(HttpStatusCode statusCode)
    {
        return ((int)statusCode >= 200) && ((int)statusCode <= 299);
    }
}

public static class CompactExtension
{
    /// <summary>
    /// Remove all double white spaces from string.
    /// Mostly used with JSON string.
    /// </summary>
    public static string Compact(this string source)
    {
        var builder = new StringBuilder(source.Length);
        bool previousWhitespace = false;

        for (int i = 0; i < source.Length; ++i)
        {
            char c = source[i];

            if (char.IsWhiteSpace(c))
            {
                previousWhitespace = true;
                continue;
            }

            if (previousWhitespace)
            {
                builder.Append(' ');
                previousWhitespace = false;
            }

            builder.Append(c);
        }

        return builder.ToString();
    }
}