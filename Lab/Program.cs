using Lab;
using Lab.Config;
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

await Email.Start();

//await Http.Start();