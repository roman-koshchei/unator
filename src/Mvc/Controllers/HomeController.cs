using Microsoft.AspNetCore.Mvc;
using Mvc.Models;
using System.Collections.Concurrent;
using System.Diagnostics;
using Unator.Extensions.Mvc;

namespace Mvc.Controllers;

public class HomeController : Controller
{
    private readonly static ConcurrentBag<Item> items = [
        new("Apple", 1), new("Banana", 56), new("Snake", 98)
    ];

    public static readonly Partial<Item> ItemPartial = new ("/Views/Home/_Item.cshtml");

    [NonAction]
    public static string Url<T>() where T : Controller
    {
        return typeof(T).Name.Replace("Controller", "");
    }

    public IActionResult Index()
    {
        return View(items);
    }

    public const string AddItemRoute = "/add-item";
    
    [HttpPost(AddItemRoute)]
    public async Task<IActionResult> AddItem([FromForm] Item item)
    {
        await Task.Delay(500);
        items.Add(item);
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
