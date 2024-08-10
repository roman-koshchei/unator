using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Mvc.Lib;
using Mvc.Models;
using System.Net.Http;

namespace Mvc.Controllers;

public class TestController(
    ViewsRender viewsRender) : Controller
{
    private readonly ViewsRender     viewsRender = viewsRender;

    public async Task< IActionResult> Index()
    {

        var item = new Item("key", 100);
        
        var view = await viewsRender.Render("/Views/Home/_Item.cshtml", item);
        
        return Ok(view);
    }

    
}
