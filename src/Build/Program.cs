using System.Text;

internal class Program
{
    private static string viewsPath;
    private static string classPath;

    private static void Main(string[] args)
    {
        viewsPath = "../../../../Mvc/Views";
        classPath = "../../../../Mvc/Lib";

        UpdateViewsClass();

        //FileSystemWatcher watcher = new()
        //{
        //    Path = viewsPath,
        //    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
        //};

        //watcher.EnableRaisingEvents = true;
    }

    private static void UpdateViewsClass()
    {
        StringBuilder sb = new();
        sb.AppendLine("using Mvc;");
        sb.AppendLine("using Mvc.Models;");
        sb.AppendLine("using Unator.Extensions.Mvc;");

        var res = GenerateClassForViews(viewsPath);
        foreach (var item in res.UsingSpaces)
        {
            sb.AppendLine($"using {item}");
        }
        sb.AppendLine(res.Content);

        File.WriteAllText(Path.Combine(classPath, "Views.cs"), sb.ToString());
    }

    public record struct ClassDescriptor(IEnumerable<string> UsingSpaces, string Content);

    public static ClassDescriptor GenerateClassForViews(string path)
    {
        HashSet<string> includes = [];

        var classSb = new StringBuilder();
        classSb.AppendLine($"public static class {Path.GetFileName(path)}");
        classSb.AppendLine("{");

        foreach (var file in Directory.EnumerateFiles(path, "*.cshtml"))
        {
            var name = Path.GetFileNameWithoutExtension(file);
            var viewPath = file.Replace(viewsPath, null).Replace('\\', '/');

            var content = File.ReadAllLines(file);
            foreach (var line in content.Where(line => line.Trim().StartsWith("@using ")))
            {
                includes.Add($"{line.Replace("@using ", "").Trim(';')};");
            }

            string partialType;
            var model = content.FirstOrDefault(line => line.Trim().StartsWith("@model "));
            if (model != null)
            {
                model = model.Replace("@model ", "").Trim(';').Trim();
                partialType = $"View<{model}>";
            }
            else
            {
                partialType = "View";
            }

            classSb.AppendLine($"public static readonly {partialType} {name} = new(\"/Views{viewPath}\");");
        }

        foreach (var dir in Directory.EnumerateDirectories(path))
        {
            var descriptor = GenerateClassForViews(dir);
            classSb.AppendLine(descriptor.Content);
            foreach (var usingSpace in descriptor.UsingSpaces)
            {
                includes.Add(usingSpace);
            }
        }

        classSb.AppendLine("}");
        return new(includes, classSb.ToString());
    }

}