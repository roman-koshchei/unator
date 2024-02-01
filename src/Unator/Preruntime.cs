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
    /// <returns>List of exceptions appeared during initialization.</returns>
    public static List<PreruntimeException> Run()
    {
        var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x
                .GetTypes()
                .Where(type => type.GetCustomAttribute<PreruntimeAttribute>() != null)
            );

        var errors = new List<PreruntimeException>();
        foreach (var type in types)
        {
            try
            {
                type.TypeInitializer?.Invoke(null, null);
            }
            catch (Exception error)
            {
                errors.Add(new PreruntimeException(type, error));
            }
        }
        return errors;
    }

    /// <summary>
    /// Prints Preruntime exceptions to console in a beautiful way.
    /// </summary>
    public static void Describe(List<PreruntimeException> errors)
    {
        Console.Write("Errors appeared during ");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("Preruntime");
        Console.ResetColor();
        Console.WriteLine(" with corresponding types:");

        Console.WriteLine();

        for (int i = 0; i < errors.Count; i++)
        {
            var error = errors[i];

            var target = PreruntimeException.Tagret(error.Type);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{target} {PreruntimeException.Name(error.Type)}");
            Console.ResetColor();

            PreruntimeException.PrintToConsole(error);

            //Console.Write(" ");
            //Console.WriteLine(error.ToString());
            Console.WriteLine();
        }
    }
}

public class PreruntimeException(Type type, Exception error) : Exception($"Exception during preruntime initialization for {Tagret(type)}: {Name(type)}", error)
{
    public Type Type { get; } = type;

    public static string Tagret(Type type)
    {
        if (type.IsClass) return "class";
        if (type.IsInterface) return "interface";
        if (type.IsValueType) return "struct";
        return "type";
    }

    public static string Name(Type type)
    {
        return type.FullName ?? type.Name;
    }

    public static void PrintToConsole(Exception error)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Error Message: {error.Message}");
        Console.ResetColor();

        // Print inner exception recursively
        if (error.InnerException != null)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("Inner Exception:");
            PrintToConsole(error.InnerException);
            Console.ResetColor();
        }

        // Print stack trace
        Console.ForegroundColor = ConsoleColor.Gray;
        Console.WriteLine($"Stack Trace: {error.StackTrace}");
        Console.ResetColor();
    }
}

/// <summary>Mark class for Preruntime static initialization.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class PreruntimeAttribute : Attribute
{
}

public static class ExceptionExtension
{
    public static void BeautifulPrint(this Exception ex)
    {
        var name = ex.GetType().Name;
    }
}