using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Unator.Http;
using static System.Net.WebRequestMethods;

namespace Lab;

/// <summary>
/// Http Server lab
///
/// Want to implement such chain:
/// -> find POST /me handler
/// -> check if auth
/// -> post me object (into db in future)
/// -> send response(me object + id)
/// -> log about me and route
/// -> something like webhook
///
/// Want to benchmark by this:
/// https://github.com/SaltyAom/bun-http-framework-benchmark
/// </summary>
///
/*

server.Loading();

server.Start();

server.Get().Send().Webhook();

server.Post().Send().Log();

server.Ready();

*/

internal class Http
{
    /// <summary>
    /// Start experiments with Unator Http Server
    /// </summary>
    /// <returns></returns>
    public static async Task Start()
    {
        UHttpServer server = new(7070);

        URouter router = new();

        router.NotFound(async ctx => await SendText(ctx, "not found"));

        int counter = 1;

        router.Get("/me", async ctx =>
        {
            await SendText(ctx, $"haha, I work: {counter}");
            Console.WriteLine($"haha, I work: {counter}");
            ++counter;
        });

        /*
            Procedural execution + functions to separate popular functionality
            The problems are:
            - if we use memory at the top of function it lives until end of function
            - we can accidentally send response twice or not at all
        */
        router.Post("/me", async ctx =>
        {
            // Auth
            var id = IsAuthorized(ctx);
            if (id == 0)
            {
                await SendText(ctx, "unauthorized");
                return;
            }

            // Logic
            string text = await ReadTextAsync(ctx);

            // Sending
            await SendText(ctx, text + id);

            // Logging
            Console.WriteLine("id: " + id);
            Console.WriteLine("text: " + text);

            // Webhook thing
            List<string> webhooks = new() { "https://github.com/roman-koshchei" };
            foreach (var webhook in webhooks) Console.WriteLine("send to webhook: " + webhook);
        });

        var result = await server.Start(ctx => _ = Task.Run(() => router.Handle(ctx)));
        //var result = await server.Start(router.Handle);

        if (result != null)
        {
            Console.WriteLine(result.Message);
        }
    }

    private static async Task SendText(HttpListenerContext ctx, string text)
    {
        var res = ctx.Response;
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        res.ContentLength64 = buffer.Length;
        await res.OutputStream.WriteAsync(buffer);
        res.Close();
    }

    private static async Task<string> ReadTextAsync(HttpListenerContext ctx)
    {
        var encoding = ctx.Request.ContentEncoding;
        using StreamReader reader = new(ctx.Request.InputStream, encoding);
        return await reader.ReadToEndAsync();
    }

    private static int IsAuthorized(HttpListenerContext ctx)
    {
        var val = new Random().Next(1);
        if (val == 1)
        {
            Console.WriteLine("Authorized");
        }
        else
        {
            Console.WriteLine("Not authorized");
            // should send response and not going next
        }
        return val;
    }
}