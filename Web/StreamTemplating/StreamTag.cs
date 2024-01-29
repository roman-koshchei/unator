namespace Web.StreamTemplating;

public class StreamTag
{
    public string Value { get; }

    public StreamTag(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Convert raw string to StringTag to sanitize them.
    /// If you want to acctually put html inside, then wrap string into RawTag.
    /// </summary>
    /// <remarks>
    /// During development to check if you correct you should comment it.
    /// Because you can accidentally sanitize tag.
    /// </remarks>
    public static implicit operator StreamTag(string value) => new TextElement(value);

    /// <summary>
    /// Hmmm, Attribute?
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator StreamTag((string key, string value) attribute)
    {
        return new HtmlAttribute($"{attribute.key}=\"{attribute.value}\"");
    }

    public async Task StreamRender(Stream stream)
    {
    }
}

/// <summary>
/// Represants value that should be sanitized.
/// </summary>
public class TextElement : StreamTag
{
    public TextElement(string value) : base(value)
    {
    }
}

/// <summary>
/// Represants value that should be sanitized.
/// </summary>
public class HtmlAttribute : StreamTag
{
    public HtmlAttribute(string value) : base(value)
    {
    }

    /// <summary>
    /// Hmmm, Attribute?
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator HtmlAttribute((string key, string value) attribute)
    {
        return new HtmlAttribute($"{attribute.key}=\"{attribute.value}\"");
    }
}