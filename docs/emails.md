# Emails

That's where things go interesting. I provide `EmailGod` class, which will manage several email services. You get benefits:

- higher chance of delivering email: 1 service fails, use another one.
- more free plan resources.

At the current moment Unator supports sending emails from:

- Resend <- the best one
- Mailjet
- SendGrid
- Brevo

`EmailGod` implements same interface as email senders. I highly recommend using at least 2 services to be unstoppable.

Code looks:

```csharp
var resendApiKey = Env.Get("RESEND_API_KEY");
var brevoApiKey = Env.Get("BREVO_API_KEY");

var emailGod = new EmailGod(
  new EmailService(new Resend(resendApiKey), new DayLimiter(100)),
  new EmailService(new Brevo(brevoApiKey), new DayLimiter(300)),
);

var emailStatus = await emailGod.Send(
  fromEmail: "example@mail.com",
  fromName: "example",
  toEmails: new List<string>() { "me@mail.com" },
  subject: "Showing EmailGod",
  text: $"It's beautiful",
  html: $"<a href='https://github.com/roman-koshchei/unator'>GitHub</a>"
);

if(emailStatus == EmailStatus.Success) {
  Console.WriteLine("Success");
}
```

We may change it in future if more functionality will be required. For example, move email
information to seperate class/struct to persist it. But it's good for now.
