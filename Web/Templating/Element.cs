namespace Web.Templating;

public class Element
{
    public string Value { get; }

    public Element(string value)
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
    public static implicit operator Element(string value) => new TextElement(value);

    /// <summary>
    /// Hmmm, Attribute?
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator Element((string key, string value) attribute)
    {
        return new AttributeElement($"{attribute.key}=\"{attribute.value}\"");
    }
}

/// <summary>
/// Represants value that should be sanitized.
/// </summary>
public class TextElement : Element
{
    public TextElement(string value) : base(value)
    {
    }
}

/// <summary>
/// Represants value that should be sanitized.
/// </summary>
public class AttributeElement : Element
{
    public AttributeElement(string value) : base(value)
    {
    }

    /// <summary>
    /// Hmmm, Attribute?
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator AttributeElement((string key, string value) attribute)
    {
        return new AttributeElement($"{attribute.key}=\"{attribute.value}\"");
    }
}