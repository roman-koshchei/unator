using System.Net;
using System.Text;

var httpListener = new HttpListener();

while (httpListener.IsListening)
{
    var ctx = await httpListener.GetContextAsync();

    ctx.Response.ContentType = "text/html";
    var bytes = Encoding.UTF8.GetBytes("<h1>Cool</h1>");
    await ctx.Response.OutputStream.WriteAsync(bytes);

    ctx.Response.Close();
}