using System.Reflection;

namespace Unator;

/// <summary>Helper to make preruntime static initialization.</summary>
public static class Preruntime
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
}

/// <summary>Mark class for Preruntime static initialization.</summary>
[AttributeUsage(AttributeTargets.Class)]
public class PreruntimeAttribute : Attribute
{
}