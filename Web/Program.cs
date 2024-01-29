using System.Net;
using Web;
using Web.Routing;

/*

    Building own http server/framework/whatever can make MVP

    Threads:
    - Main thread looks after other threads
    - HTTP listener thread (try multiple ones, so it's like load balancing)
    - Backgroud Queue thread
    - DB pool thread ?

    Threads will communicate between each other through Channels
*/

var productRoute = U.Route(
    path: U.Path("admin/shops").Param<int>().Path("products"),
    handler: async (shopId, ctx) =>
    {
        ctx.Response.StatusCode = 200;
        await ctx.Response.Html($"<h1>admin/shops/{shopId}/products</h1><p>{shopId} is int</p>");
    }
);

var productStrRoute = U.Route(
    path: U.Path("admin/shops").Param<string>().Path("products"),
    handler: async (shopId, ctx) =>
    {
        ctx.Response.StatusCode = 200;
        await ctx.Response.Html($"<h1>admin/shops/{shopId}/products</h1><p>{shopId} is string</p>");
    }
);

var otherRoute = U.Route(
    path: U.Path("other").Param<int>().Path("some/products"),
    handler: async (shopId, ctx) =>
    {
        ctx.Response.StatusCode = 200;
        await ctx.Response.Html($"<h1>other/{shopId}/some/products</h1>");
    }
);

IURoute[] routes = { productRoute, productStrRoute, otherRoute };

HttpServer httpServer = new(8080);
await httpServer.Run(async (ctx) =>
{
    var req = ctx.Request;
    var res = ctx.Response;

    if (req.Url == null)
    {
        Console.WriteLine("There isn't url");
        return;
    }

    var path = req.Url.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
    var handledSuccessfully = false;
    for (int i = 0; i < routes.Length && handledSuccessfully == false; i += 1)
    {
        var route = routes[i];
        handledSuccessfully = await route.TryRun(path, ctx);
    }

    if (handledSuccessfully == false)
    {
        await res.Html("<h1>Not found</h1>");

        await Bg.Enqueue(async () =>
        {
            await Task.Delay(10000);
            Console.WriteLine("Bg job");
        });
    }

    res.Close();
});

//QueryParser(Handle);

//static void Handle([Query] int search, HttpListenerContext ctx)
//{
//    Console.WriteLine($"search: {search}");
//}

//static Func<string, string> QueryParser(Delegate del)
//{
//    MethodInfo methodInfo = del.Method;

//    string[] parameterNames = methodInfo
//        .GetParameters()
//        .Where(p => p.GetCustomAttribute<QueryAttribute>() != null)
//        .Select(p => p.Name ?? "No name")
//        .ToArray();

//    foreach (string parameter in parameterNames)
//    {
//        Console.WriteLine($"Param: {parameter}");
//    }
//}

[AttributeUsage(AttributeTargets.Parameter)]
public class QueryAttribute : Attribute
{
}

//// admin/shops/{shopId}/products/{productId}
//// with such a code we can get a url builder function

//var productsRoute = RoutePath.Path("admin/shops").Int().Path("products");
//var url = productsRoute.Url(1);

//// somewhere in html:
//var link = App.AdminShopProducts.Url();

//public static class App
//{
//    public static Route AdminShopProducts { get; } = Route.New(
//        RoutePath.Path("admin/shops").Int().Path("products"),
//        (route, ctx) =>
//        {
//            Console.WriteLine(route);

//            ctx.Response.StatusCode = 200;
//            ctx.Response.Close();
//        }
//    );
//}

//public static class Routes
//{
//    public static RoutePath<string> AdminShopProducts { get; } = RoutePath.Path("admin/shops").String().Path("products");
//}

//public interface IOORoute
//{
//    public IRoutePath Path();

//    public Task Handle(HttpListenerContext ctx);
//}

//public class OORoute : IOORoute
//{
//    public IRoutePath Path() => RoutePath.Path("admin/shops").Int().Path("products");

//    public Task Handle(HttpListenerContext ctx)
//    {
//        Console.WriteLine(ctx.Request.RawUrl);

//        ctx.Response.StatusCode = 200;
//        ctx.Response.Close();

//        return Task.CompletedTask;
//    }
//}