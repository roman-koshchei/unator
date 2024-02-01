using Unator;

var errors = Preruntime.Run();
if (errors.Count > 0)
{
    Preruntime.Describe(errors);
    return;
}

Console.WriteLine($"Environment variable for UNATOR: {SECRETS.UNATOR}");

[Preruntime]
public static class SECRETS
{
    public static readonly string UNATOR = Env.GetRequired("UNATOR");
}