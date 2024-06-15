using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using System.Diagnostics;
using Unator.Extensions.Mvc;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }


    public static readonly Partial<ItemModel> ItemPartial = new ("/Views/Home/_Item.cshtml"); 

    public IActionResult Index()
    {
        var item = new ItemModel("room", 216);
        return ItemPartial.Result(this, item);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
