using Lab;
using Lab.Config;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using System.Text;
using Unator;
using Unator.Http;
using static System.Net.Mime.MediaTypeNames;

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

public static class CompactExtension
{
    /// <summary>
    /// Remove all white spaces from string.
    /// Mostly used with JSON string.
    /// </summary>
    public static string Compact(this string source)
    {
        var builder = new StringBuilder(source.Length);
        bool previousWhitespace = false;

        for (int i = 0; i < source.Length; ++i)
        {
            char c = source[i];

            if (char.IsWhiteSpace(c))
            {
                previousWhitespace = true;
                continue;
            }

            if (previousWhitespace)
            {
                builder.Append(' ');
                previousWhitespace = false;
            }

            builder.Append(c);
        }

        return builder.ToString();
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