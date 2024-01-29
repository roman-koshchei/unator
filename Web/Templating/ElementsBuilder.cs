using System.Text;
using System.Web;

namespace Web.Templating;

public static partial class El
{
    private static string SanitizeForHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return HttpUtility.HtmlEncode(input);
    }

    private static Element ClosingTag(string name, params Element[] content)
    {
        StringBuilder tagBuilder = new($"<{name}");
        StringBuilder contentBuilder = new();

        for (int i = 0; i < content.Length; ++i)
        {
            var tag = content[i];
            _ = tag switch
            {
                TextElement => contentBuilder.Append(SanitizeForHtml(tag.Value)),
                AttributeElement => tagBuilder.Append(' ').Append(tag.Value),
                _ => contentBuilder.Append(tag.Value)
            };
        }
        tagBuilder.Append('>').Append(contentBuilder).Append($"</{name}>");

        return new Element(tagBuilder.ToString());
    }

    private static Element SelfClosingTag(string name, params AttributeElement[] attributes)
    {
        StringBuilder tagBuilder = new($"<{name}");
        for (int i = 0; i < attributes.Length; ++i)
        {
            var attribute = attributes[i];
            tagBuilder.Append(' ').Append(attribute.Value);
        }
        tagBuilder.Append("/>");

        return new Element(tagBuilder.ToString());
    }

    public static Element For<T>(IEnumerable<T> list, Func<T, Element> builder)
    {
        StringBuilder sb = new();
        foreach (var item in list) sb.Append(builder(item).Value);
        return new Element(sb.ToString());
    }

    public static Element Merge(IEnumerable<Element> list)
    {
        StringBuilder sb = new();
        foreach (var item in list) sb.Append(item.Value);
        return new Element(sb.ToString());
    }

    public static string Html(params Element[] content)
    {
        //StringBuilder sb = new("<!DOCTYPE html><html>");
        StringBuilder sb = new("<html>");
        for (int i = 0; i < content.Length; ++i)
        {
            var tag = content[i];
            sb.Append(tag is TextElement ? SanitizeForHtml(tag.Value) : tag.Value);
        }
        sb.Append("</html>");
        return sb.ToString();
    }
}