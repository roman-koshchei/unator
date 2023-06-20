namespace Unator.Email;

/// <summary>
/// Allow to send maximum amount of emails.
/// Add as much as possible of email sending services.
/// And then get as much emails as possible for lowest price
/// </summary>
public class EmailGod : UEmailSender
{
    private readonly List<EmailService> services;

    /// <summary>
    /// Require at least 1 sender.
    /// </summary>
    /// <param name="service">Required sender</param>
    /// <param name="services">All other senders</param>
    public EmailGod(EmailService service, params EmailService[] services)
    {
        this.services = new List<EmailService>(services.Length + 1) { service };
        this.services.AddRange(services);
    }

    public async Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        try
        {
            for (int i = 0; i < services.Count; ++i)
            {
                var service = services[i];

                if (service.Limiters.All(l => l.IsLimitAllow()))
                {
                    var error = await service.Sender.SendOne(from, to, subject, html);

                    if (error == null)
                    {
                        foreach (var limiter in service.Limiters) limiter.IncrementLimiter();
                        return null;
                    }
                    return error;
                }
            }
            return new LimitReachedException();
        }
        catch (Exception ex)
        {
            return ex;
        }
    }
}