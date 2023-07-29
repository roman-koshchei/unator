using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Email;

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
public class EmailGod : IEmailSender
{
    private readonly IImmutableList<EmailService> services;

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