using Mvc;
using Mvc.Models;
using Unator.Extensions.Mvc;
using Mvc;
using Mvc.Models;
using Mvc.Controllers;
public static class Views
{
public static readonly View _ViewImports = new("/Views/_ViewImports.cshtml");
public static readonly View _ViewStart = new("/Views/_ViewStart.cshtml");
public static class Home
{
public static readonly View<IEnumerable<Item>> Index = new("/Views/Home/Index.cshtml");
public static readonly View Privacy = new("/Views/Home/Privacy.cshtml");
public static readonly View<Item> _Item = new("/Views/Home/_Item.cshtml");
}

public static class Shared
{
public static readonly View<ErrorViewModel> Error = new("/Views/Shared/Error.cshtml");
public static readonly View _Layout = new("/Views/Shared/_Layout.cshtml");
public static readonly View _ValidationScriptsPartial = new("/Views/Shared/_ValidationScriptsPartial.cshtml");
}

}

