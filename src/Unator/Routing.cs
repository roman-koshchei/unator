namespace Unator;

public static class RoutingExample
{
    public static async Task Run()
    {
        //var productRoute = U.Route(
        //    path: U.Path("admin/shops").Param<int>().Path("products"),
        //    handler: async (shopId, ctx) =>
        //    {
        //        ctx.Response.StatusCode = 200;
        //        var str = $"<h1>admin/shops/{shopId}/products</h1><p>{shopId} is int</p>";
        //        ctx.Response.OutputStream.Write(Encoding.UTF8.GetBytes(str));
        //    }
        //);

        //var productStrRoute = U.Route(
        //    path: U.Path("admin/shops").Param<string>().Path("products"),
        //    handler: async (shopId, ctx) =>
        //    {
        //        ctx.Response.StatusCode = 200;
        //        var str = $"<h1>admin/shops/{shopId}/products</h1><p>{shopId} is string</p>";
        //        ctx.Response.OutputStream.Write(Encoding.UTF8.GetBytes(str));
        //    }
        //);

        //IURoute[] routes = { productRoute, productStrRoute };

        //var handledSuccessfully = false;
        //for (int i = 0; i < routes.Length && handledSuccessfully == false; i += 1)
        //{
        //    var route = routes[i];
        //    //handledSuccessfully = await route.TryRun();
        //}
    }
}

public class U
{
    public static UPath Path(string path) => UPath.New().Path(path);

    public static UPath<T> Param<T>() where T : IParsable<T> => UPath<T>.New();

    public static URoute<U, T> Route<U, T>(UPath<T> path, Func<T, U, Task> handler) where T : IParsable<T>
    {
        return new() { Path = path, Handler = handler };
    }
    public static URoute<T> Route<T>(UPath path, Func<T, Task> handler) 
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

public interface IURoute<T>
{
    public Task<bool> TryRun(IEnumerable<string> path, T ctx);
}

public class URoute<T> : IURoute<T>
{
    public UPath Path { get; init; }
    public Func<T, Task> Handler { get; init; }

    public async Task<bool> TryRun(IEnumerable<string> pathSegments, T ctx)
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

public class URoute<T, Param1> : IURoute<T> where Param1 : IParsable<Param1>
{
    public required UPath<Param1> Path { get; init; }
    public required Func<Param1, T, Task> Handler { get; init; }


    public async Task<bool> TryRun(IEnumerable<string> pathSegments, T ctx)
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
            param = Param1.Parse(pathSegments.ElementAt(i), null);
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

    public UPath<T> Param<T>() where T : IParsable<T> => UPath<T>.New(Segments);

    public UPath Path(string path)
    {
        var pathSegments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
        return New(Segments.Concat(pathSegments));
    }
}

public class UPath<Param1> where Param1 : IParsable<Param1>
{
    public IEnumerable<string> BeforeParam { get; private set; } = Array.Empty<string>();
    public IEnumerable<string> AfterParam { get; private set; } = Array.Empty<string>();

    //public UPath<Param1, Param2> Param<Param2>() where Param2 : IConvertible => new() { Val = $"{Val}/{typeof(Param2).Name}" };

    public string Url(Param1 param1)
    {
        return $"{param1}";
    }

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