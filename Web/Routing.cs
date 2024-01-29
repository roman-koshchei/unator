using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace Web.Routing;

// U

public class U
{
    public static UPath Path(string path) => UPath.New().Path(path);

    public static UPath<T> Param<T>() where T : IConvertible => UPath<T>.New();

    public static URoute<T> Route<T>(UPath<T> path, Func<T, HttpListenerContext, Task> handler) where T : IConvertible
    {
        return new() { Path = path, Handler = handler };
    }

    //public static URoute<T, U> Route<T, U>(
    //    UPath<T, U> path, Func<T, U, HttpListenerContext, Task> handler
    //) where T : IConvertible where U : IConvertible
    //{
    //    return new() { Path = path, Handler = handler };
    //}
}

// ROUTE

public interface IURoute
{
    public Task<bool> TryRun(IEnumerable<string> path, HttpListenerContext ctx);
}

public class URoute : IURoute
{
    public UPath Path { get; init; }
    public Func<HttpListenerContext, Task> Handler { get; init; }

    public async Task<bool> TryRun(IEnumerable<string> pathSegments, HttpListenerContext ctx)
    {
        var count = pathSegments.Count();
        if (Path.Segments.Count() != count) return false;

        for (int i = 0; i < count; i++)
        {
            if (Path.Segments.ElementAt(i) != pathSegments.ElementAt(i)) return false;
        }

        await Handler(ctx);
        return true;
    }
}

public class URoute<Param1> : IURoute where Param1 : IConvertible
{
    public UPath<Param1> Path { get; init; }
    public Func<Param1, HttpListenerContext, Task> Handler { get; init; }

    public async Task<bool> TryRun(IEnumerable<string> pathSegments, HttpListenerContext ctx)
    {
        // some/path/:param/another/path

        var count = Path.BeforeParam.Count() + 1 + Path.AfterParam.Count();
        if (count != pathSegments.Count()) return false;

        int i = 0;
        foreach (var segment in Path.BeforeParam)
        {
            if (segment != pathSegments.ElementAt(i)) return false;
            i += 1;
        }

        Param1 param;
        try
        {
            param = (Param1)Convert.ChangeType(pathSegments.ElementAt(i), typeof(Param1));
            i += 1;
        }
        catch
        {
            return false;
        }

        foreach (var segment in Path.AfterParam)
        {
            if (segment != pathSegments.ElementAt(i)) return false;
            i += 1;
        }

        await Handler(param, ctx);
        return true;
    }
}

//public class URoute<Param1, Param2> where Param1 : IConvertible where Param2 : IConvertible
//{
//    public UPath<Param1, Param2> Path { get; init; }
//    public Func<Param1, Param2, HttpListenerContext, Task> Handler { get; init; }
//}

// PATH

public class UPath
{
    public IEnumerable<string> Segments { get; private set; } = Array.Empty<string>();

    public static UPath New(IEnumerable<string> val) => new() { Segments = val };

    public static UPath New() => new();

    public UPath<T> Param<T>() where T : IConvertible => UPath<T>.New(Segments);

    public UPath Path(string path)
    {
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return New(Segments.Concat(pathSegments));
    }
}

public class UPath<Param1> where Param1 : IConvertible
{
    public IEnumerable<string> BeforeParam { get; private set; } = Array.Empty<string>();
    public IEnumerable<string> AfterParam { get; private set; } = Array.Empty<string>();

    //public UPath<Param1, Param2> Param<Param2>() where Param2 : IConvertible => new() { Val = $"{Val}/{typeof(Param2).Name}" };

    public static UPath<Param1> New(IEnumerable<string> pathBeforeParam) => new() { BeforeParam = pathBeforeParam };

    public static UPath<Param1> New() => new();

    public UPath<Param1> Path(string path)
    {
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        var upath = UPath<Param1>.New(BeforeParam);
        upath.AfterParam = AfterParam.Concat(pathSegments);
        return upath;
    }
}

//public class UPath<Param1, Param2> where Param1 : IConvertible where Param2 : IConvertible
//{
//    public string Val { get; set; } = string.Empty;

//    public UPath<Param1, Param2> Path(string path) => new() { Val = $"{Val}/{path}" };
//}