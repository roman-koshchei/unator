using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unator.Email;

/// <summary>
/// Allow to send maximum amount of emails.
/// Add as much as possible of email sending services.
/// And then get as much emails as possible for lowest price
/// </summary>
public class EmailGod
{
    private readonly List<UEmailSender> senders;

    /// <summary>
    /// Require at least 1 sender.
    /// </summary>
    /// <param name="sender">Required sender</param>
    /// <param name="senders">All other senders</param>
    public EmailGod(UEmailSender sender, params UEmailSender[] senders)
    {
        this.senders = new List<UEmailSender>(senders.Length + 1) { sender };
        this.senders.AddRange(senders);
        this.senders.Sort((a, b) => a.GetMonthLimit().CompareTo(b.GetMonthLimit()));
    }

    public async Task<Exception?> Send(string from, string to, string subject, string html)
    {
        try
        {
            for (int i = 0; i < senders.Count; ++i)
            {
                var sender = senders[i];
                if (sender.IsLimitAllow())
                {
                    await sender.SendEmail(from, to, subject, html);
                    return null;
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