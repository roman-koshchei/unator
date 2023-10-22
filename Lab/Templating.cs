using System.Text.RegularExpressions;
using Unator;

namespace Lab;

public record struct User(string Name);

[Preruntime]
public static class Templates
{
    public static readonly Func<User, string> UserTemplate = TemplatingEngine.Build<User>(@"
        @props Lab.User

        <!-- This is comment. It will be removed. -->

        <h1>Template Engine with Preruntime type checks.</h1>
        <h2>User name:  @props.Name</h2>

        @use UserCard(@props)

        @if props.Name.StartsWith(""Roman Koshchei"")
            Roman Koshchei is super cool.
        @else if props.Name.StartsWith(""Prime"")
            Are you actually here Primeagen?
        @else
            You are a fool.
        @endif

        @each props.Name as part, i
            @var num = i + 1
            <p>@num @part</p>
        @endeach

        @component UserCard
        @props Lab.User
            <div>
                User: <h3>@props.Name</h3>
            </div>
        @endcomponent
    ");
}

internal static class TemplatingEngine
{
    private const string props = "@props";

    /// <summary> Check HTML correctness. Return minimized HTML string. </summary>
    public static string Build(string template)
    {
        return template.Compact();
    }

    public static Func<Props, string> Build<Props>(string template)
    {
        template = RemoveHtmlComments(template);

        var lines = template.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        var modelLine = lines.FirstOrDefault(x => x.StartsWith(props));
        if (modelLine == null) throw new Exception("Props are not found isn't found");

        var modelName = modelLine[(props.Length + 1)..].Trim();
        if (typeof(Props).FullName != modelName) throw new Exception($"Template provided model: {modelName}, but code provided: {typeof(Props).FullName}");

        return (Props props) => $"Temporary. Props: {props}. Template: {template}";
    }

    private static string RemoveHtmlComments(string html)
    {
        string commentsRemoved = Regex.Replace(html, "<!--(.*?)-->", string.Empty);

        // Split the string into lines
        string[] lines = commentsRemoved.Split('\n');

        // Remove empty lines and leading/trailing whitespace from each line
        string cleanedHtml = string.Join("\n", lines
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line)));

        return cleanedHtml;
    }
}