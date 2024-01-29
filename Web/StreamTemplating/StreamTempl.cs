using System;
using System.IO.Compression;
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Web.StreamTemplating;

public static partial class ST
{
    private static string SanitizeForHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return HttpUtility.HtmlEncode(input);
    }

    private static StreamTag ClosingTag(string name, params StreamTag[] content)
    {
        StringBuilder tagBuilder = new($"<{name}");
        StringBuilder contentBuilder = new();

        for (int i = 0; i < content.Length; ++i)
        {
            var tag = content[i];
            _ = tag switch
            {
                TextElement => contentBuilder.Append(SanitizeForHtml(tag.Value)),
                HtmlAttribute => tagBuilder.Append(' ').Append(tag.Value),
                _ => contentBuilder.Append(tag.Value)
            };
        }
        tagBuilder.Append('>').Append(contentBuilder).Append($"</{name}>");

        return new StreamTag(tagBuilder.ToString());
    }

    private static StreamTag SelfClosingTag(string name, params HtmlAttribute[] attributes)
    {
        StringBuilder tagBuilder = new($"<{name}");
        for (int i = 0; i < attributes.Length; ++i)
        {
            var attribute = attributes[i];
            tagBuilder.Append(' ').Append(attribute.Value);
        }
        tagBuilder.Append("/>");

        return new StreamTag(tagBuilder.ToString());
    }

    //public static StreamTag For<T>(IEnumerable<T> list, Func<T, StreamTag> builder)
    //{
    //    StringBuilder sb = new();
    //    foreach (var item in list) sb.Append(builder(item).Value);
    //    return new StreamTag(sb.ToString());
    //}

    //public static StreamTag Merge(IEnumerable<StreamTag> list)
    //{
    //    StringBuilder sb = new();
    //    foreach (var item in list) sb.Append(item.Value);
    //    return new StreamTag(sb.ToString());
    //}

    public static async Task Html(Stream stream, params StreamTag[] content)
    {
        //StringBuilder sb = new("<!DOCTYPE html><html>");
        await stream.WriteString("<html>");
        for (int i = 0; i < content.Length; ++i)
        {
            var tag = content[i];
            var inner = tag is TextElement ? SanitizeForHtml(tag.Value) : tag.Value;
            await stream.WriteString(inner);
        }
        await stream.WriteString("</html>");
    }

    public static StreamTag Button(params StreamTag[] content)
    {
        Console.WriteLine("Button");
        return new StreamTag("button");
    }

    public static StreamTag Img(params HtmlAttribute[] attributes)
    {
        StringBuilder tagBuilder = new($"<img");
        for (int i = 0; i < attributes.Length; ++i)
        {
            var attribute = attributes[i];
            tagBuilder.Append(' ').Append(attribute.Value);
        }
        tagBuilder.Append("/>");

        return new StreamTag(tagBuilder.ToString());
    }

    public static async Task WriteString(this Stream stream, string text)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        await stream.WriteAsync(buffer, 0, buffer.Length);
    }
}

public static class StreamTemplTest
{
    public static async Task Test()
    {
        var stream = new MemoryStream();

        var html = $@"
            <html>
                <button>
                    <img src='/me.png'/>
                </button>
            </html>
        ";

        await ST.Html(stream,
            ST.Img(("high", "")),
            ST.Button(
                ST.Img(("src", "/me.png"))
            )
        );
    }
}