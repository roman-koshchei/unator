using Lab;
using Lab.Config;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;
using Unator.Http;

/*

    Lab = laboratory
    Test all different things
    See how they work

*/

var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
Env.LoadFromFile(dotenv);

Secrets.BrevoApiKey = Env.Get("BREVO_API_KEY");
Secrets.ResendApiKey = Env.Get("RESEND_API_KEY");
Secrets.MailjetApiKey = Env.Get("MAILJET_API_KEY");
Secrets.MailjetSecret = Env.Get("MAILJET_SECRET_KEY");
Secrets.PostmarkApiKey = Env.Get("POSTMARK_API_KEY");

await Email.Start();

//await Http.Start();