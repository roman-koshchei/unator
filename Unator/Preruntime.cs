using System.Reflection;

namespace Unator;

/// <summary>
/// Make static initialization for classes marked with [Preruntime].
/// Used if you want to initialize static fields at the start of the program.
/// Can be useful if you have different functions based on environment you work with,
/// but don't want to check it on every execution.
/// </summary>
public static class Preruntime
{
    /// <summary>
    /// Run static initialization of all classes marked with [Preruntime].
    /// </summary>
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