using System.IO;
using System.Text;
using Unator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<Loader>();

var app = builder.Build();

app.UseHttpsRedirection();

app.Use(async (ctx, next) =>
{
    try
    {
        var loader = new Loader();

        loader.Service.Get.PrintServiceUsageCount();

        ctx.Response.StatusCode = 200;
        ctx.Response.ContentType = "text/html";
        var bytes = Encoding.UTF8.GetBytes(
            "<html><head></head><body><h1>Unalytics</h1></body></html>"
        );
        await ctx.Response.BodyWriter.WriteAsync(bytes);
    }
    catch
    {
        await next();
    }
});

app.Run();

// Folder DB

public class Loader
{
    public CachedService<ExampleService> Service { get; } = new();
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