using System.Text;

namespace Web;

public interface ICss
{
    protected void Css(string styles);
}

public class Html : ICss
{
    private readonly StringBuilder css = new();
    private readonly StringBuilder head = new();

    protected void Css(string styles)
    {
        css.Append(styles);
    }

    protected void Head(string head)
    {
        this.head.Append(head);
    }

    public string Layout(string children)
    {
        return @$"
        <!DOCTYPE html>
        <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                {head}
                <style>{css}</style>
            </head>
            <body>{children}</body>
            </html>
        ";
    }

    public static string Div(string children) => $"<div>{children}</div>";

    void ICss.Css(string styles)
    {
        throw new NotImplementedException();
    }
}

public partial class Pages : Html
{
    public string Landing()
    {
        int num = new Random().Next();

        return Layout(
            Carousel(num)
        );
    }
}

public partial class Pages : Html
{
    public string Carousel(int count)
    {
        for (int i = 0; i < count; i++)
        {
            Css($"#item-{i}:hover {{ padding: {i * 4} rem; }}");
        }

        return Div("Carousel");
    }
}