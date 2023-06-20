using System.Net;

namespace Unator.Http;

public class UHttpServer
{
    private readonly HttpListener listener = new();
    private readonly string url;
    private bool run = false;

    public UHttpServer(int port)
    {
        url = $"http://localhost:{port}/";
        listener.Prefixes.Add(url);
    }

    public async Task<Exception?> Start(Func<HttpListenerContext, Task> handler)
    {
        Exception? result = null;
        try
        {
            run = true;
            listener.Start();
            Console.WriteLine($"Server started at: {url}");
            while (run)
            {
                var ctx = await listener.GetContextAsync();
                await handler(ctx);
            }
        }
        catch (Exception ex)
        {
            result = ex;
        }
        finally
        {
            listener.Stop();
            listener.Close();
        }
        return result;
    }
}