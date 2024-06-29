using Asp.Routes;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using Unator;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();


var app = builder.Build();

app.UseHttpsRedirection();
app.MapHealthChecks("/healthz");

app.MapGet("/views", () =>
{
});

var landing = U.Route(
    path: U.Path("/"),
    handler: async (HttpContext ctx) =>
    {
        ctx.Response.StatusCode = 200;
        ctx.Response.ContentType = "text/html";
        await ctx.Response.WriteAsync($@"
<html>
    <head>
        <title>Unator Asp Lab</title>
        <script>
            function time() {{ return 5; }}
        </script>
    </head>
    <body>
        <button
            unalytics-on=""mouseover""
            unalytics-once
            unalytics-event=""Button hover""
            unalytics-data=""{{ count: time() }}""
        >
            Click
        </button>
    </body>
</html>");

        ProductRoute.Url(432);
    }
);

var shopSettings = U.Route(IRoutes.ProductPath, async (ShopId shopId, HttpContext ctx) =>
{
    Console.WriteLine(IRoutes.ProductPath.Url(4));
    ctx.Response.StatusCode = 200;
});

var customersRoute = U.Route(IRoutes.CustomerDetails, async (int customerId, HttpContext ctx) =>
{
    ctx.Response.StatusCode = 200;
    ctx.Response.Headers.ContentType = "text/html";
    var url = IRoutes.CustomerDetails.Url(56);
    await ctx.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(url));
});

app.Use(async (ctx, next) =>
{
    var segments = ctx.Request.Path.ToString().Split('/', StringSplitOptions.RemoveEmptyEntries);
    var found = await landing.TryRun(segments, ctx);
    if (found is false)
    {
        await next(ctx);
    }
});

app.Run();

public struct ShopId : IParsable<ShopId>
{
    public required int Value { get; set; }

    public static implicit operator int(ShopId shopId) => shopId.Value;

    public static implicit operator ShopId(int value) => new ShopId { Value = value };

    public static ShopId Parse(string s, IFormatProvider? provider)
    {
        return new ShopId { Value = int.Parse(s) };
    }

    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, [MaybeNullWhen(false)] out ShopId result)
    {
        try
        {
            result = new ShopId { Value = int.Parse(s ?? "0") };
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }

    public override string? ToString()
    {
        return Value.ToString();
    }
}

public interface IRoutes
{
    public static readonly UPath AppGroup = U.Path("app");

    public static readonly UPath<ShopId> ProductPath = U.Path("shops").Param<ShopId>().Path("settings");

    static readonly UPath<int> CustomerDetails = AppGroup.Path("customers").Param<int>();
}

public interface Route<Param, State> where Param : IParsable<Param>
{
    public abstract static UPath<Param> Path { get; }

    public abstract static string Url(Param param);

    public Task Handle(State state, Param param);
}

internal interface AspRoute<Param> : Route<Param, HttpContext> where Param : IParsable<Param>
{ }

public class Loader
{
    public CachedService<ExampleService> Service { get; } = new();
    public CachedService<AnnoyingService> AnnoyingService { get; } = new();
}

public class ExampleService
{
    private int count = 1;

    public void PrintServiceUsageCount()
    {
        Console.WriteLine($"Service used {count} times");
        count += 1;
    }
}

public class AnnoyingService
{
    private int count = 1;

    public void Annoy()
    {
        for (int i = 0; i < count; i++)
        {
            Console.WriteLine("AAAA");
        }

        count += 1;
    }
}