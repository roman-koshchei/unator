using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Mvc.Lib;

public class ViewsRender : IDisposable
{
    private readonly IServiceScope scope;
    private readonly IRazorViewEngine viewEngine;
    private readonly ActionContext actionContext;
    private readonly TempDataDictionary tempData;

    private static readonly RouteData routeData = new();
    private static readonly ActionDescriptor actionDescriptor = new();
    private static readonly EmptyModelMetadataProvider emptyModelMetadataProvider = new();
    private static readonly ModelStateDictionary modelStateDictionary = new();

    public ViewsRender(IRazorViewEngine viewEngine, ITempDataProvider tempDataProvider, IServiceScopeFactory scopeFactory)
    {
        this.viewEngine = viewEngine;
        scope = scopeFactory.CreateScope();

        var httpContext = new DefaultHttpContext { RequestServices = scope.ServiceProvider };
        actionContext = new ActionContext(httpContext, routeData, actionDescriptor);
        tempData = new TempDataDictionary(httpContext, tempDataProvider);
    }

    public async Task<string> Render<T>(string path, T model)
    {
        var viewResult = viewEngine.GetView(".", path, false);
        if (!viewResult.Success)
        {
            throw new InvalidOperationException($"Couldn't find view '{path}'");
        }

        var viewDictionary = new ViewDataDictionary(emptyModelMetadataProvider, modelStateDictionary)
        {
            Model = model
        };

        using var sw = new StringWriter();
        var viewContext = new ViewContext(
            actionContext, viewResult.View, viewDictionary,
            tempData, sw, new HtmlHelperOptions()
        );

        await viewResult.View.RenderAsync(viewContext);

        string[] parts = sw.ToString().Replace("\r", "").Replace("\n", "")
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);

        return string.Join(" ", parts).Replace("> <", "><");
    }

    public void Dispose()
    {
        scope.Dispose();
    }
}