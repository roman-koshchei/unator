using System.Text;

namespace Unator.Extensions.Routing;

public class UnatorRoute(string pattern)
{
    private readonly string pattern = pattern;

    public UnatorRoute Add(string path)
    {
        return new($"{pattern.TrimEnd('/')}/{path.TrimStart('/')}");
    }

    public UnatorRoute<string> Param(string name) => new(pattern, name, "");

    public UnatorRoute<T> Param<T>(string name) => new(pattern, name, "");

    public string Pattern => pattern;

    public string Url() => $"/{pattern.Trim('/')}";

    public static UnatorRoute New(string pattern) => new(pattern);
}

public class UnatorRoute<TParam1>(string start, string paramName, string end)
{
    private readonly string start = start.TrimEnd('/');
    private readonly string param = paramName.Trim('/');
    private readonly string end = end.Trim('/');

    public UnatorRoute<TParam1> Add(string path)
    {
        return new(start, param, $"{end.TrimEnd('/')}/{path.TrimStart('/')}");
    }

    public string Pattern => $"{start}/{{{param}}}/{end}";

    public string Url(TParam1 param) => $"{start}/{param}/{end}";

    public UnatorRoute<TParam1, TParam2> Param<TParam2>(string name) => new(start, param, end, name, "");
}

public class UnatorRoute<TParam1, TParam2>(
    string start, string param1, string middle, string param2, string end
)
{
    private readonly string start = start.TrimEnd('/');
    private readonly string param1 = param1.Trim('/');
    private readonly string middle = middle.Trim('/');
    private readonly string param2 = param2.Trim('/');
    private readonly string end = end.Trim('/');

    public string Pattern
    {
        get
        {
            StringBuilder sb = new($"{start}/{{{param1}}}");
            if (!string.IsNullOrWhiteSpace(middle)) sb.Append($"/{middle}");
            sb.Append($"/{{{param2}}}");
            if (!string.IsNullOrWhiteSpace(end)) sb.Append($"/{end}");
            return sb.ToString();
        }
    }

    public string Url(TParam1 param1, TParam2 param2)
    {
        StringBuilder sb = new($"{start}/{param1}");
        if (!string.IsNullOrWhiteSpace(middle)) sb.Append($"/{middle}");
        sb.Append($"/{param2}");
        if (!string.IsNullOrWhiteSpace(end)) sb.Append($"/{end}");
        return sb.ToString();
    }
}