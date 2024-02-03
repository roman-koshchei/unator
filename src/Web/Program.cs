using System.Net;
using Unator;
using static Unator.EnvInteresting;

//EnvExample.Run();

var email = new EmailSwitch(
    new EmailService(new Resend("resend-api-token"), new DayLimiter(100)),
    new EmailService(new Brevo("brevo-api-token"), new DayLimiter(300)),
    new EmailService(new SendGrid("send-grid-api-token"), new DayLimiter(100))
);

// In future I plan create an email builder
// So it will be easy to create text and html versions at the same time
// For example Header("Hi me from flurium account!")

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