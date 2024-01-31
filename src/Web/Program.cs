using Unator;

var errors = Preruntime.Run();
if (errors.Count > 0)
{
    Preruntime.Describe(errors);
    return;
}

Console.WriteLine("Running program if ok");

[Preruntime]
public static class Secrets
{
    static Secrets()
    {
        var UNATOR = Env.GetRequired("UNATOR");
    }
}