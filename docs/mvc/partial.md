# Partial

MVC gives us an option to create Partial Views, which are something like Components.
But here is a problem: we don't have only 1 place of truth about partial view.

If we want to render partial, then we need to specify path and pass model (props) value

```cshtml
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

```cshtml
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

```cs
public class HomeController {

  public static readonly Partial<Item> ItemPartial = new("/Views/Home/_Item.cshtml");


}

```
