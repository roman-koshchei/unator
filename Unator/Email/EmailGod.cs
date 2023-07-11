using System.Collections.Immutable;

namespace Unator.Email;

/// <summary>
/// Allow to send maximum amount of emails.
/// Add as much as possible of email sending services.
/// And then get as much emails as possible for lowest price
/// </summary>
public class EmailGod : UEmailSender
{
    private readonly IImmutableList<EmailService> services;

    /// <summary>
    /// Require at least 1 sender.
    /// </summary>
    /// <param name="service">Required sender</param>
    /// <param name="services">All other senders</param>
    public EmailGod(EmailService service, params EmailService[] services)
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