﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Unator.Templating;

public class UI
{
    public static Tag Label => new("label");
    public static Tag Button => new("button");
    public static Tag Small => new("small");
    public static Tag Form => new("form");
    public static ATag A => new("a");
    public static Tag H1 => new("h1");
    public static Tag H2 => new("h2");
    public static Tag Hgroup => new("hgroup");
    public static Tag Div => new("div");
    public static Tag Textarea => new("textarea");
    public static Tag Select => new("select");
    public static Tag Option => new("option");
    public static Tag P => new("p");
    public static Tag Input => new("input");
    public static Tag Footer => new("footer");
    public static Tag Fieldset => new("fieldset");
    public static Tag Blockquote => new("blockquote");

    public static Tag Table => new("table");
    public static Tag Thead => new("thead");
    public static Tag Tbody => new("tbody");
    public static Tag Tr => new("tr");
    public static Tag Th => new("th");
    public static Tag Td => new("td");

    public static string Text(string text) => HttpUtility.HtmlEncode(text);

    public static string Many(params string[] tags)
    {
        return string.Join(null, tags);
    }

    public static string Many(IEnumerable<string> tags)
    {
        return string.Join(null, tags);
    }
}

public class Tag(string tag) : Tag<Tag>(tag)
{
}

public class Tag<T>(string tag) where T : Tag<T>
{
    protected readonly List<(string, string?)> attributes = [];

    public T Attr(string name, string value)
    {
        attributes.Add((name, value));
        return (T)this;
    }

    public T Flag(string name)
    {
        attributes.Add((name, null));
        return (T)this;
    }

    public T Role(string role)
    {
        attributes.Add(("role", role));
        return (T)this;
    }

    public T Id(string id)
    {
        attributes.Add(("id", id));
        return (T)this;
    }

    public T Style(string style)
    {
        attributes.Add(("style", style));
        return (T)this;
    }

    public T Class(string className)
    {
        attributes.Add(("class", className));
        return (T)this;
    }

    public T Type(string value)
    {
        attributes.Add(("type", value));
        return (T)this;
    }

    public T Name(string value)
    {
        attributes.Add(("name", value));
        return (T)this;
    }

    public T Placeholder(string value)
    {
        attributes.Add(("placeholder", value));
        return (T)this;
    }

    public override string ToString()
    {
        return Wrap("");
    }

    public string this[params string[] content]
    {
        get => Wrap(content);
    }

    public string Wrap(params string[] content)
    {
        var sb = new StringBuilder();
        sb.Append('<');
        sb.Append(tag);
        sb.Append(' ');
        foreach (var attr in attributes)
        {
            if (attr.Item2 is null)
            {
                sb.Append(attr.Item1);
            }
            else
            {
                sb.Append($"{attr.Item1}='{attr.Item2}'");
            }
        }
        sb.Append('>');

        foreach (var item in content)
        {
            sb.Append(item);
        }
        sb.Append($"</{tag}>");
        return sb.ToString();
    }

    public static implicit operator string(Tag<T> tag) => tag.Wrap();
}

public class ATag(string tag) : Tag<ATag>(tag)
{
    public ATag Href(string url)
    {
        attributes.Add(("href", url));
        return this;
    }

    public ATag Target(string target)
    {
        attributes.Add(("target", target));
        return this;
    }

    public ATag Blank()
    {
        attributes.Add(("target", "_blank"));
        return this;
    }
}