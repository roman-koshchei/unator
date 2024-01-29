using System.Text;

namespace Web.Templating;

using System.Text;

public static partial class El
{
    public static Element A(params Element[] content) => ClosingTag("a", content);

    public static Element Article(params Element[] content) => ClosingTag("article", content);

    public static Element Aside(params Element[] content) => ClosingTag("aside", content);

    public static Element B(params Element[] content) => ClosingTag("b", content);

    public static Element Blockquote(params Element[] content) => ClosingTag("blockquote", content);

    public static Element Body(params Element[] content) => ClosingTag("body", content);

    public static Element Br(params AttributeElement[] attributes) => SelfClosingTag("br", attributes);

    public static Element Button(params Element[] content) => ClosingTag("button", content);

    public static Element Code(params Element[] content) => ClosingTag("code", content);

    public static Element Details(params Element[] content) => ClosingTag("details", content);

    public static Element Dialog(params Element[] content) => ClosingTag("dialog", content);

    public static Element Div(params Element[] content) => ClosingTag("div", content);

    public static Element Footer(params Element[] content) => ClosingTag("footer", content);

    public static Element Form(params Element[] content) => ClosingTag("form", content);

    public static Element H1(params Element[] content) => ClosingTag("h1", content);

    public static Element H2(params Element[] content) => ClosingTag("h2", content);

    public static Element H3(params Element[] content) => ClosingTag("h3", content);

    public static Element H4(params Element[] content) => ClosingTag("h4", content);

    public static Element H5(params Element[] content) => ClosingTag("h5", content);

    public static Element H6(params Element[] content) => ClosingTag("h6", content);

    public static Element Head(params Element[] content) => ClosingTag("head", content);

    public static Element Header(params Element[] content) => ClosingTag("header", content);

    public static Element I(params Element[] content) => ClosingTag("i", content);

    public static Element Iframe(params Element[] content) => ClosingTag("iframe", content);

    public static Element Img(params AttributeElement[] attributes) => SelfClosingTag("img", attributes);

    public static Element Input(params AttributeElement[] attributes) => SelfClosingTag("input", attributes);

    public static Element Kbd(params Element[] content) => ClosingTag("kbd", content);

    public static Element Label(params Element[] content) => ClosingTag("label", content);

    public static Element Li(params Element[] content) => ClosingTag("li", content);

    public static Element Main(params Element[] content) => ClosingTag("main", content);

    public static Element Meta(params AttributeElement[] attributes) => SelfClosingTag("meta", attributes);

    public static Element Nav(params Element[] content) => ClosingTag("nav", content);

    public static Element Ol(params Element[] content) => ClosingTag("ol", content);

    public static Element Optgroup(params Element[] content) => ClosingTag("optgroup", content);

    public static Element Option(params Element[] content) => ClosingTag("option", content);

    public static Element P(params Element[] content) => ClosingTag("p", content);

    public static Element Pre(params Element[] content) => ClosingTag("pre", content);

    public static Element Progress(params Element[] content) => ClosingTag("progress", content);

    public static Element Script(params Element[] content) => ClosingTag("script", content);

    public static Element Section(params Element[] content) => ClosingTag("section", content);

    public static Element Select(params Element[] content) => ClosingTag("select", content);

    public static Element Small(params Element[] content) => ClosingTag("small", content);

    public static Element Span(params Element[] content) => ClosingTag("span", content);

    public static Element Strong(params Element[] content) => ClosingTag("strong", content);

    public static Element Link(params AttributeElement[] attributes) => SelfClosingTag("link", attributes);

    public static Element Style(params Element[] content) => ClosingTag("style", content);

    public static Element Summary(params Element[] content) => ClosingTag("summary", content);

    public static Element Svg(params Element[] content) => ClosingTag("svg", content);

    public static Element Table(params Element[] content) => ClosingTag("table", content);

    public static Element Tbody(params Element[] content) => ClosingTag("tbody", content);

    public static Element Td(params Element[] content) => ClosingTag("td", content);

    public static Element Textarea(params Element[] content) => ClosingTag("textarea", content);

    public static Element Tfoot(params Element[] content) => ClosingTag("tfoot", content);

    public static Element Th(params Element[] content) => ClosingTag("th", content);

    public static Element Thead(params Element[] content) => ClosingTag("thead", content);

    public static Element Title(params Element[] content) => ClosingTag("title", content);

    public static Element Tr(params Element[] content) => ClosingTag("tr", content);

    public static Element Ul(params Element[] content) => ClosingTag("ul", content);
}