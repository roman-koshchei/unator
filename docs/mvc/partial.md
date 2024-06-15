# Partial

MVC gives us an option to create Partial Views, which are something like Components.
But here is a problem: we don't have only 1 place of truth about partial view.

If we want to render partial, then we need to specify path and pass model (props) value

```html
@await Html.PartialAsync("/Views/Shared/_Card.cshtml", "some value");
```

But if we use this in many places and then for some reason we need to change path or model type,
then it will take much time. And you can miss usage or component somewhere and get bugs.

That's why I created `Partial` class. Here are example of usage:

1. Item.cs - some class we will work around

```cs
public record Item(string Name, int Number);
```

2. \_Item.cshtml - partial view for single item.

```html
@model Item;

<div class="col">
  <div class="card">
    <div class="card-body">
      <h3 class="card-title">@Model.Name</h3>
      <p class="card-text">@Model.Number</p>
    </div>
  </div>
</div>
```

3. HomeController.cs - controller where we will handle logic and keep partials

```cs
public class HomeController
{
  // some store of all items
  public static ConcurrentBag<Item> Items { get; } = [];

  // define single source of truth about _Item partial
  public static readonly Partial<Item> ItemPartial = new("/Views/Home/_Item.cshtml");

  // home page with a simple list of items
  public IActionResult Index()
  {
    return View(Items);
  }

  // simple post request for htmx, which returns partial view
  [HttpPost("/add-item")]
  public async Task<IActionResult> AddItem([FromForm] Item item)
  {
    Items.Add(item);
    return ItemPartial.Result(this, item);
  }
}
```

4. Index.cshtml - regular page view, where I demonstrate how to render Partial in Views. As well it contains simple HTMX form to demonstrate usage of Partial as result of Http Post operation.

```html
@model IEnumerable<Item>
  <form hx-post="/add-item" hx-target="#items" hx-swap="afterbegin">
    <input type="text" name="@nameof(Item.Name)" required />
    <input type="number" name="@nameof(Item.Number)" value="0" />
    <button type="submit">Submit</button>
  </form>

  <div id="#items">
    @foreach(var item in Model) { @await HomeController.ItemPartial.Render(Html,
    item); }
  </div></Item
>
```
