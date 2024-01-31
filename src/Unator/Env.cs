namespace Unator;

/// <summary>
/// Helper to work with environment variables.
/// </summary>
public static class Env
{
    /// <summary>
    /// Load environment variables from file if file exists.
    /// If file doesn't exist then it just does nothing.
    /// <param name="path">Path to environment file.</param>
    /// </summary>
    public static void LoadFile(string path)
    {
        if (!File.Exists(path)) return;

        foreach (var line in File.ReadAllLines(path))
        {
            var separatorIndex = line.IndexOf('=');

            var key = line[..separatorIndex].Trim();
            var value = line[(separatorIndex + 1)..].Trim();

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    /// <summary>
    /// Get required environment variable, throw if variable isn't found.
    /// </summary>
    /// <exception cref="KeyNotFoundException"/>
    public static string GetRequired(string key)
         => Environment.GetEnvironmentVariable(key)
        ?? throw new KeyNotFoundException($"Environment variable '{key}' wasn't found!");

    public static T GetRequired<T>(string key) where T : IParsable<T>
    {
        var value = GetRequired(key);
        try
        {
            return T.Parse(value, null);
        }
        catch (Exception ex)
        {
            throw new FormatException($"Environment variable '{key}' can't be parsed into {nameof(T)} type!", ex);
        }
    }

    /// <summary>Get optional environment variable.</summary>
    /// <returns>Null if not found, otherwise value</returns>
    public static string? GetOptional(string key) => Environment.GetEnvironmentVariable(key);

    /// <summary>Get optional environment variable of class <typeparamref name="T"/>.</summary>
    /// <returns>Null if not found, otherwise value</returns>
    public static T? GetOptionalRef<T>(string key) where T : class, IParsable<T>
    {
        var value = GetOptional(key);
        if (value == null) return null;
        try
        {
            return T.Parse(value, null);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>Get optional environment variable of value struct <typeparamref name="T"/>.</summary>
    /// <returns>Null if not found, otherwise value</returns>
    public static T? GetOptionalVal<T>(string key) where T : struct, IParsable<T>
    {
        var value = GetOptional(key);
        if (value == null) return null;
        try
        {
            return T.Parse(value, null);
        }
        catch
        {
            return null;
        }
    }
}