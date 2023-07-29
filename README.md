![unator banner](./assets/unator-banner.png)

# Unator

Breaking mistaken standards of enterprise such as Repository pattern.
My goal is to provide a set of functions/classes to make development transparent and logical.

## Database / Entity Framework Core

Probably you use Entity Framework Core to access database. I do so.
But you may be taught to use `Repository` pattern and throw exceptions. Not anymore!
I too few benefits to add complexity of Repository.

So we will use just few generic extensions for database. And our code will look like this:

```csharp
var products = await db.Products
  .Where(x => x.Price < 100)
  .Select(x => new { x.Id, x.Title })
  .QueryMany();

var product = await db.Products
  .Select(x => new { x.Id, x.Title })
  .QueryOne(x => x.Id == 51);
```

`QueryMany` and `QueryOne` allow to get data. You can pass where condition right into function or do it before. Both functions are configured with `ConfigureAwait(false)`. When you query one entity it returns null if entity isn't found.

The benefit is you remove complexity and query minimal amount of data by using `Select`.

```csharp
var product = new Product("Title for new product");
await db.Products.AddAsync(product)
var saved = await db.Save();
```

`Save` for DbContext allow us to save changes to the database without throwing exceptions. It just returns `true` if changes saved successfully otherwise `false`.
