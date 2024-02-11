using Microsoft.AspNetCore.Mvc;
using Unator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<Loader>();
builder.Services.AddScoped<ExampleService>();
builder.Services.AddScoped<AnnoyingService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    var startTime = DateTime.UtcNow;

    await next.Invoke();

    var endTime = DateTime.UtcNow;

    var processingTime = endTime - startTime;
    Console.WriteLine($"Request {context.Request.Path} time: {processingTime.Ticks} ticks");
});

app.MapGet("/a", ([FromServices] Loader l) =>
{
    l.Service.Get.PrintServiceUsageCount();
    l.Service.Get.PrintServiceUsageCount();
    l.Service.Get.PrintServiceUsageCount();
    l.AnnoyingService.Get.Annoy();
    l.AnnoyingService.Get.Annoy();
    l.AnnoyingService.Get.Annoy();
    return Results.Ok();
});

app.MapGet("/b", ([FromServices] ExampleService ex, [FromServices] AnnoyingService ann) =>
{
    ex.PrintServiceUsageCount();
    ex.PrintServiceUsageCount();
    ex.PrintServiceUsageCount();
    ann.Annoy();
    ann.Annoy();
    ann.Annoy();
    return Results.Ok();
});

app.Run();

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