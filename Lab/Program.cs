using System.Text;
using Unator;

/*

    Lab = laboratory
    Test all different things
    See how they work

*/

var error = Preruntime.Run();
if (error != null) throw error;

Console.WriteLine("tscrt");
Console.WriteLine(Repo.Query());

[Preruntime]
public static class Repo
{
    public static readonly Func<string> Query = Preruntime.Make("SELECT * FROM me");
}

/*
var root = Directory.GetCurrentDirectory();
var dotenv = Path.Combine(root, ".env");
Env.LoadFromFile(dotenv);

Secrets.BrevoApiKey = Env.Get("BREVO_API_KEY");
Secrets.ResendApiKey = Env.Get("RESEND_API_KEY");
Secrets.MailjetApiKey = Env.Get("MAILJET_API_KEY");
Secrets.MailjetSecret = Env.Get("MAILJET_SECRET_KEY");
Secrets.PostmarkApiKey = Env.Get("POSTMARK_API_KEY");
*/

//await Email.Start();

//await Http.Start();