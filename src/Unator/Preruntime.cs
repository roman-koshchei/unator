using System.Reflection;
using System.Text;

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

            error.BeautifulPrint(4);
        }
    }
}

public class PreruntimeException(Type type, Exception error)
    : Exception($"Preruntime exception for {Tagret(type)}: {Name(type)}", error)
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
}

/// <summary>Mark class for Preruntime static initialization.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
public class PreruntimeAttribute : Attribute
{
}

public static class ExceptionExtension
{
    public static void BeautifulPrint(this Exception ex, int leftPadding = 0)
    {
        Console.CursorLeft = leftPadding;
        Console.WriteLine("Error:");

        var i = 0;
        var current = ex;
        while (current != null)
        {
            Console.CursorLeft = leftPadding + 4;
            var name = current.GetType().Name;

            Console.Write($"{i}: ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(current.Message);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($" - {name}");
            Console.ForegroundColor = ConsoleColor.White;

            current = current.InnerException;
            i += 1;
        }

        List<string> traces = [];
        i = 0;
        current = ex;
        while (current != null)
        {
            if (current.StackTrace != null)
            {
                var currentTraces = current.StackTrace.Split(Environment.NewLine);
                foreach (var trace in currentTraces)
                {
                    traces.Add($"{i}: {trace.TrimStart(' ', 'a', 't')}");
                    i += 1;
                }
            }

            current = current.InnerException;
        }

        if (traces.Count > 0)
        {
            Console.CursorLeft = leftPadding;
            Console.WriteLine("Stack Trace:");
            foreach (var trace in traces)
            {
                Console.CursorLeft = leftPadding + 4;
                Console.WriteLine(trace);
            }
        }

        Console.WriteLine();
    }
}