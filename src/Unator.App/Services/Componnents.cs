using Unator.Templating;

namespace Unator.App.Services;

public class Componnents
{
    public static string Input(
          string name, string type, string label, string placeholder, bool isRequired, string? error, string? value = null
    )
    {
        var input = UI.Input.Type(type).Placeholder(placeholder)
            .Name(name).Attr("aria-describedby", $"{name}-helper");
        if (isRequired) { input.Flag("required"); }
        if (value is not null) { input.Attr("value", value); }

        return UI.Label[
            UI.Text(label),
            UI.Input.Type(type).Placeholder(placeholder),
            error != null ? UI.Small.Id($"{name}-helper")[error] : ""
        ];
    }

    public static SplitElement BaseLayout(string title, string description = "", string head = "") => new(@$"
        <!DOCTYPE html><html lang='en' data-theme='light'>
        <head>
            <meta charset='utf-8' />
            <meta name='viewport' content='width=device-width, initial-scale=1.0' />
            <link rel='stylesheet' href='/pico.pumpkin.min.css' />
            <link rel='stylesheet' href='/styles.css' />

            <title>{title}</title>
            <meta name='description' content='{description ?? "Leave your Feedback right here, so we know what the heck in your mind"}' />

            <script src='/scripts/wave.js' defer></script>

            {head}
        </head>
        <body style='overflow: auto scroll; padding: 1rem'>",
@"</body></html>");
}