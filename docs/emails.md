# Email switch

I bet you need to send emails in almost any kind of projects. SaaS or just marketing page with "subscribe to newsletter".

And that's where things go interesting.

I provide `EmailSwitch` class, which will manage several email services. You get benefits:

- higher chance of delivering email: if 1 service fails, use another one
- more free plan resources ;)

Code example:

```csharp
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
```

`EmailSwitch` implements same interface as email senders. I highly recommend using at least 2 services to be unstoppable.

## Support

|          | Link                                 | Tested |
| -------- | ------------------------------------ | ------ |
| Resend   | [resend.com](https://resend.com)     | yes    |
| SendGrid | [sendgrid.com](https://sendgrid.com) | yes    |
| Brevo    | [brevo.com](https://www.brevo.com/)  | yes    |

## Inspiration

I saw [Hyperswitch](https://hyperswitch.io/) on a trending list in GitHub long time ago and kept an eye on it.
It's a switch for payments. Basically allowing support of different payments methods and processors.

While developing [Spentoday](https://www.spentoday.com/), I decided to do same thing but for emails. It was a StartUp and I wanted to spend 0 money on infrastructure. So I just used free limits from Resend, Send Grid and Brevo to send our emails.

We actually didn't need so much, but surely EmailSwitch will be useful in future.

## Templating

As you have seen above in a Code Example in `Send` method there is argument `html`.
So you can send not only text, but html as well if email client supports showing html.

Writing 2 versions: text and html by yourself is kind of annoying. Moreover not all html and css is supported in emails.

So in future I plan to create an Email Templating inspired by [React Email](https://react.email/). React Email gives you components that will generate html suitable for email clients.
