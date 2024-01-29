using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using Web.StreamTemplating;

namespace Web;

public class HttpServer
{
    private readonly string url;

    public HttpServer(int port = 7777)
    {
        url = $"http://localhost:{port}/";
    }

    public async Task Run(Func<HttpListenerContext, Task> handler)
    {
        HttpListener listener = new() { Prefixes = { url } };

        listener.Start();

        Console.WriteLine($"Http Server started listening: {url}");

        try
        {
            while (listener.IsListening)
            {
                HttpListenerContext ctx = await listener.GetContextAsync();
                _ = handler(ctx);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
        finally
        {
            listener.Stop();
        }
    }
}

public static class HttpServerExtension
{
    public static async Task Html(this HttpListenerResponse res, string html)
    {
        res.ContentType = "text/html";
        res.ContentEncoding = Encoding.UTF8;
        await res.OutputStream.WriteString(html);
    }
}