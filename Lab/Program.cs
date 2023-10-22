/*

    Lab = laboratory
    Test all different things
    See how they work

*/

using Lab;
using Unator;

var error = Preruntime.Run();
Console.WriteLine(error);

var page = Templates.UserTemplate(new User("ROMAN KOSHCHEI"));
Console.WriteLine(page);

public class PreruntimeFunc
{
    public static Func<T, Task> IfDev<T>(Func<T, Task> action)
    {
        var env = Env.GetOptional("ASPNETCORE_ENVIRONMENT");
        if (env == "Development") return action;
        return (T input) => Task.CompletedTask;
    }

    public static readonly Func<(string name, string surname), Task> Log = IfDev(
        ((string name, string surname) input) =>
        {
            Console.WriteLine($"Hello {input.name} {input.surname}");
            return Task.CompletedTask;
        }
    );

    public async Task Test()
    {
        var name = "Roman";
        var surname = "Koshchei";

        await Log((name, surname));
    }
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