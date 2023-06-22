namespace Unator.Email.Senders;

public class Mailchimp : UEmailSender
{
    private const string url = "https://api.mailjet.com/v3.1/send";

    public Task<Exception?> SendOne(string from, string to, string subject, string html)
    {
        throw new NotImplementedException();
    }
}