![unator banner](./assets/unator-banner.png)

# Unator

Breaking mistaken standards of enterprise such as Repository pattern.
My goal is to provide a set of functions/classes to make development transparent and logical.

## Repository

That's my favorite. You create a ton of files for Repositories and files with interfaces for them,
even if it has just 1 method. You should not forget to add them to DI (that's what my team did several times).
And almost all of them are so similar. Let's take a look:

```csharp
// ICategoryRepository.cs
public interface ICategoryRepository : IRepository<Category>
{
  Task<Category?> GetById(int id);
  Task<Category?> Update(int id, string name);
}

// CategoryRepository.cs
public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
{
  public async Task<Category?> GetById(int id) {
    return await Entities
      .Include(c => c.Lots)
      .FirstOrDefaultAsync(c => c.Id == id)
      .ConfigureAwait(false);
  }

  public async Task<Category?> Update(int id, string name)
  {
    var category = await Entities
      .FirstOrDefaultAsync(c => c.Id == id)
      .ConfigureAwait(false);
    if (category != null) {
      category.Name = name;
      await _db.SaveChangesAsync();
    }
    return category;
  }
}

// CategoryController.cs
public async Task<IResult> TestQuery(int id)
{
  var category = categoryRepository.GetById(id);
  if(category == null) return Results.NotFound();
  return Results.Ok(category);
}

public async Task<IResult> TestEdit(int id, string name)
{
  try {
    var category = categoryRepository.Update(id, name);
    if(category == null) return Results.NotFound();
    return Results.Ok(category);
  } catch (Exception) {
    return Results.Problem();
  }
}
```

Pretty massive if you remember you should do it for almost every table in a database. So what is proposed by Unator?

```csharp
// UQueryExtension.cs - universal for all tables
public static class UQueryExtension
{
  public static async Task<T?> QueryOne<T>(
    this IQueryable<T> query,
    Expression<Func<T, bool>> condition
  )
  {
    return await query
      .FirstOrDefaultAsync(condition)
      .ConfigureAwait(false);
  }
}

// UMutationExtension.cs - universal for all tables
public record MutationResult<T>(T? Data, Exception? Error) where T : class;

public static class UMutationExtension
{
  public static async Task<MutationResult<T>> EditOne<T>(
    this DbContext db,
    Expression<Func<T, bool>> condition,
    Action<T> mutation
  ) where T : class
  {
    try
    {
      var dbSet = db.Set<T>();
      var entity = await dbSet.FirstOrDefaultAsync(condition).ConfigureAwait(false);
      if (entity == null) throw new EntityNotFoundException();
      mutation(entity);
      await db.SaveChangesAsync().ConfigureAwait(false);
      return new(entity, null);
    }
    catch (Exception ex) { return new(null, ex); }
  }
}

// CategoryController.cs
// AppDbContext db
public async Task<IResult> TestQuery(int id)
{
  var category = await db.Categories.Include(c => c.Lots).QueryOne(c => c.Id == id);
  if(category == null) return Results.NotFound();
  return Results.Ok(category);
}

public async Task<IResult> TestEdit(int id, string name)
{
  var result = await db.EditOne<Category>(
    c => c.Id == id,
    c => c.Name = name
  );

  if (result.Data != null) return Results.Ok(result.Data);
  if (result.Error is EntityNotFoundException) return Results.NotFound();
  return Results.Problem();
}
```

UQueryExtension makes it more clear, just providing smaller functions with ConfigureAwait(false). UMutationExtension looks strange and big at first. But remember that it's universal to all tables and you don't touch it a lot. We also do return Exception as a possible value not throwing it. In my opinion, there should not be a layer between data and logic, DbContext is already a good enough abstraction, Unator just makes it more beautiful.
