﻿/*

    Lab = laboratory
    Test all different things
    See how they work

*/

using Unator;

var result = Maybe.Result(Load, ("beac", "tscri"));

if (result.Data != null)
{
}

static string Load((string env, string path) input)
{
    throw new NotImplementedException();
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