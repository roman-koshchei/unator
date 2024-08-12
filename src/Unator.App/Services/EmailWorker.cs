using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using Unator.App.Data;

namespace Unator.App.Services;

public interface IUnatorEmailSender
{
    /// <exception cref="Exception">Method can throw exception, so handle it!</exception>
    Task SendEmail(
        string fromName, string fromEmail,
        string subject, string contact,
        string receiverEmail, CancellationToken cancellationToken
    );
}

public class MockupEmailSender : IUnatorEmailSender
{
    private const int AverageSmtpServerResponseTime = 200;

    private ulong sendEmailCounter = 0;

    public async Task SendEmail(
        string fromName, string fromEmail,
        string subject, string contact,
        string receiverEmail, CancellationToken cancellationToken
    )
    {
        var currentCounter = Interlocked.Read(ref sendEmailCounter);
        if (currentCounter >= 10)
        {
            Interlocked.Exchange(ref sendEmailCounter, 0);
            throw new Exception("Can't send email exception");
        }

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"{receiverEmail} {subject}");
        Console.ResetColor();

        await Task.Delay(AverageSmtpServerResponseTime, cancellationToken);
        Interlocked.Increment(ref sendEmailCounter);
    }
}

public record struct Receiver(string ContactId, string Email);

public class EmailWorker(IServiceProvider provider, ILogger<EmailWorker> logger) : BackgroundService
{
    private static readonly ConcurrentQueue<(DbEmail, Queue<Receiver>)> emails = new();

    public static void Push(DbEmail email, Queue<Receiver> receivers)
    {
        emails.Enqueue((email, receivers));
    }

    private static async Task EnqueuePendingEmails(UnatorDb db, CancellationToken cancellationToken)
    {
        // TODO: mark email as fully sent
        // TODO: optimize, currently it's super heavy on everything
        var emails = await db.Emails.AsNoTracking().ToArrayAsync(cancellationToken);
        foreach (var email in emails)
        {
            // optimize queue creation
            var receivers = await db.Receivers
                .AsNoTracking()
                .Where(x => x.EmailId == email.Id)
                .Include(x => x.Contact)
                .OrderBy(x => x.Contact.Created)
                .Select(x => new Receiver(x.ContactId, x.Contact.Email))
                .ToArrayAsync(cancellationToken);

            if (receivers.Length > 0)
            {
                Push(email, new(receivers));
            }
        }
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<UnatorDb>();
        IUnatorEmailSender emailSender = new MockupEmailSender();

        await EnqueuePendingEmails(db, cancellationToken);

        List<(string, List<string>)> failedToDeleteReceivers = [];

        while (cancellationToken.IsCancellationRequested is false)
        {
            if (emails.TryDequeue(out var value))
            {
                List<string> failedToDeleteContactIds = [];

                var (email, receivers) = value;
                while (receivers.TryDequeue(out var receiver) && cancellationToken.IsCancellationRequested is false)
                {
                    try
                    {
                        await emailSender.SendEmail(
                            email.FromName, email.FromEmail,
                            email.Subject, email.Content,
                            receiver.Email, cancellationToken
                        );
                    }
                    catch
                    {
                        // TODO: may cause infinite queue?
                        receivers.Enqueue(receiver);
                        logger.LogError("Can't send email to {email}.", receiver.Email);
                        continue; // dangerouse continue
                    }

                    try
                    {
                        _ = await db.Receivers
                            .Where(x => x.EmailId == email.Id && x.ContactId == receiver.ContactId)
                            .ExecuteDeleteAsync(CancellationToken.None);

                        //await DeletePendingEmailFromDb(db, email);
                    }
                    catch
                    {
                        failedToDeleteContactIds.Add(receiver.ContactId);
                        logger.LogCritical("Email was sent, but not deleted from database.");
                    }
                }

                if (failedToDeleteContactIds.Count > 0)
                {
                    failedToDeleteReceivers.Add((email.Id, failedToDeleteContactIds));
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("No email");
                Console.ResetColor();
                await Task.Delay(3000, cancellationToken);
            }
        }

        if (failedToDeleteReceivers.Count > 0)
        {
            try
            {
                foreach (var (emailId, contactIds) in failedToDeleteReceivers)
                {
                    foreach (var contactId in contactIds)
                    {
                        DbReceiver receiver = new() { EmailId = emailId, ContactId = contactId };
                        db.Entry(receiver).State = EntityState.Deleted;
                    }
                }
                _ = await db.SaveChangesAsync(CancellationToken.None);
            }
            catch
            {
                logger.LogError("Retry to delete failed to delete receivers is failed");
            }
        }
    }
}