using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Unator.Extensions.Mvc;

/// <summary>
/// Class to represent partial view in code.
/// Provides single source of truth, like path to file and model type.
/// </summary>
/// <typeparam name="T">Model type, like props</typeparam>
/// <param name="Path">Path to view file (.cshtml)</param>
public record Partial<T>(string Path)
{
    /// <summary>
    /// Provides IActionResult that can be returned from controller.
    /// Used inside of controllers.
    /// </summary>
    public PartialViewResult Result(Controller controller, T model)
    {
        return controller.PartialView(Path, model);
    }

    /// <summary>
    /// Renders partial in Views.
    /// </summary>
    public Task<IHtmlContent> Render(IHtmlHelper htmlHelper, T model)
    {
        return htmlHelper.PartialAsync(Path, model);
    }
}

