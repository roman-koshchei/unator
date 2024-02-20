using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web;

/// <summary>
/// Email Templating
///
/// https://react.email/docs/components/container
/// https://demo.react.email/preview/vercel-invite-user.tsx?view=source&lang=jsx
/// https://jsx.email/docs/components/conditional
/// </summary>
public static class Email
{
    public static IElement Empty() => new Empty();

    public static IElement Html(params IElement[] children)
    {
        return new HtmlElement(children);
    }

    public static LinkElement Link(string href, string text)
    {
        return new(href, text);
    }

    public static IElement Text(string style = "", params IElement[] children)
    {
        return new TextElement(children);
    }

    public static IElement H1(string text) => new HeadingElement(1, text);

    public static IElement H2(string text) => new HeadingElement(2, text);

    public static IElement H3(string text) => new HeadingElement(3, text);

    /// <summary>
    /// Recommendation: keep it under 90 symbols
    /// </summary>
    public static PreviewElement Preview(string text) => new(text);
}

public interface IElement
{
    public void RenderText(StringBuilder sb);

    public void RenderHtml(StringBuilder sb);
}

public struct TextElement(IElement[] children) : IElement
{
    public void RenderHtml(StringBuilder sb)
    {
        sb.Append("<p>");
        foreach (var child in children)
        {
            child.RenderHtml(sb);
        }
        sb.Append("</p>");
    }

    public void RenderText(StringBuilder sb)
    {
        sb.Append('\n');
        foreach (var child in children)
        {
            child.RenderHtml(sb);
        }
        sb.Append('\n');
    }
}

public struct HtmlElement(IElement[] children) : IElement
{
    public void RenderHtml(StringBuilder sb)
    {
        sb.Append("<!DOCTYPE html PUBLIC \"-//W3C//DTD XHTML 1.0 Transitional//EN\" \"http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd\">");
        sb.Append("<html>");
        foreach (var child in children)
        {
            child.RenderHtml(sb);
        }
        sb.Append("</html>");
    }

    public void RenderText(StringBuilder sb)
    {
        foreach (var child in children)
        {
            child.RenderHtml(sb);
        }
    }
}

public struct LinkElement(string href, string text) : IElement
{
    public void RenderHtml(StringBuilder sb)
    {
        sb.Append($"<a href=\"{href}\">{text}</a>");
    }

    public void RenderText(StringBuilder sb)
    {
        sb.Append($"{text} {href}");
    }
}

public struct Empty : IElement
{
    public void RenderHtml(StringBuilder sb)
    { }

    public void RenderText(StringBuilder sb)
    { }
}

public struct HeadingElement(int level, string text) : IElement
{
    public void RenderHtml(StringBuilder sb)
    {
        sb.Append($"<h{level}>{text}</h{level}>");
    }

    public void RenderText(StringBuilder sb)
    {
        sb.Append($"{new string('#', level)} {text}\n");
    }
}

public class PreviewElement(string text) : IElement
{
    public void RenderHtml(StringBuilder sb)
    {
        sb.Append(
            $"<div style=\"display:none;overflow:hidden;line-height:1px;opacity:0;max-height:0;max-width:0\">{text}</div>"
        );
    }

    public void RenderText(StringBuilder sb)
    {
    }
}