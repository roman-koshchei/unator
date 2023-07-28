using System.Reflection;

namespace Unator;

public class Preruntime
{
    /// <summary>Run static initialization of all classes marked with [Preruntime].</summary>
    /// <returns>Null if success otherwise Exception.</returns>
    public static Exception? Run()
    {
        try
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var classes = assembly
                .GetTypes()
                .Where(type => type.GetCustomAttribute<PreruntimeAttribute>() != null);

            foreach (var type in classes) type.TypeInitializer?.Invoke(null, null);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    public static Func<string> Make(string query)
    {
        var result = query.ToLower();
        return () =>
        {
            Console.WriteLine(result);
            return result;
        };
    }
}

/// <summary>Mark class for Preruntime static initialization.</summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class PreruntimeAttribute : Attribute
{
}