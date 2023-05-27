using System.Net;

namespace Unator.Http;

/// <summary>
/// All the router should do is to find proper handler according to url
/// </summary>
public class URouter
{
    private readonly Dictionary<string, Func<HttpListenerContext, Task>> handlers = new();
    private Func<HttpListenerContext, Task>? notFoundHandler = null;

    public void Get(string route, Func<HttpListenerContext, Task> handler) => handlers.Add($"get{route}", handler);

    public void Post(string route, Func<HttpListenerContext, Task> handler) => handlers.Add($"post{route}", handler);

    public void Patch(string route, Func<HttpListenerContext, Task> handler) => handlers.Add($"patch{route}", handler);

    public void Put(string route, Func<HttpListenerContext, Task> handler) => handlers.Add($"put{route}", handler);

    public void Delete(string route, Func<HttpListenerContext, Task> handler) => handlers.Add($"delete{route}", handler);

    public void NotFound(Func<HttpListenerContext, Task> handler)
    {
        notFoundHandler = handler;
    }

    public void Route(string method, string route, Func<HttpListenerContext, Task> handler) => handlers.Add($"{method.ToLower()}{route}", handler);

    public async Task Handle(HttpListenerContext ctx)
    {
        var route = $"{ctx.Request.HttpMethod.ToLower()}{ctx.Request.Url?.AbsolutePath}";
        var handler = GetHandler(route);
        if (handler != null)
        {
            await handler(ctx);
            return;
        }

        if (notFoundHandler != null)
        {
            await notFoundHandler(ctx);
            return;
        }
    }

    private Func<HttpListenerContext, Task>? GetHandler(string route)
    {
        handlers.TryGetValue(route, out var handler);
        return handler;
    }
}