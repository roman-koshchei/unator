namespace Lab.Config;

/// <summary>
/// Load environment variables from {something}.env file.
/// Used during development to not store environment variables in repository.
/// </summary>
public static class Env
{
    public static void LoadFromFile(string filePath)
    {
        if (!File.Exists(filePath)) return;

        foreach (var line in File.ReadAllLines(filePath))
        {
            var separatorIndex = line.IndexOf("=");

            var key = line.Substring(0, separatorIndex).Trim();
            var value = line.Substring(separatorIndex + 1).Trim();

            Environment.SetEnvironmentVariable(key, value);
        }
    }

    public static string Get(string key) => Environment.GetEnvironmentVariable(key)!.Trim();
}