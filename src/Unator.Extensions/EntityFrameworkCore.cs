using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Unator.Extensions;

/// <summary>
/// Extend IQueryable to ConfigureAwait(false) by default.
/// </summary>
public static class QueryExtension
{
    /// <summary>Query one/first item with ConfigureAwait(false)</summary>
    /// <returns>Item if found otherwise null.</returns>
    public static async Task<T?> QueryOne<T>(this IQueryable<T> query)
    {
        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Query one/first item that matches <paramref name="where"/> condition with ConfigureAwait(false)
    /// </summary>
    /// <param name="where">Condition to filter.</param>
    /// <returns>Item if found otherwise null.</returns>
    public static async Task<T?> QueryOne<T>(this IQueryable<T> query, Expression<Func<T, bool>> where)
    {
        return await query.FirstOrDefaultAsync(where).ConfigureAwait(false);
    }

    /// <summary>
    /// Query many items that match where condition with ConfigureAwait(false)
    /// </summary>
    /// <returns>List of item.</returns>
    public static async Task<IEnumerable<T>> QueryMany<T>(this IQueryable<T> query)
    {
        return await query.ToListAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Query many items that match <paramref name="where"/> condition with ConfigureAwait(false)
    /// </summary>
    /// <param name="where">Condition to filter.</param>
    /// <returns>List of items</returns>
    public static async Task<IEnumerable<T>> QueryMany<T>(this IQueryable<T> query, Expression<Func<T, bool>> where)
    {
        return await query.Where(where).ToListAsync().ConfigureAwait(false);
    }
}

public static class DbExtension
{
    /// <summary>Save changes in database. But doesn't throw.</summary>
    /// <returns>True if successful, false if not.</returns>
    public static async Task<bool> SafeSave(this DbContext db)
    {
        try
        {
            await db.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// AddRangeAsync to database table, but checks if values count > 0.
    /// If count == 0, then default AddRangeAsync will throw an exception.
    /// </summary>
    public static async Task SafeAddRange<T>(
        this DbSet<T> table, IEnumerable<T> values,
        CancellationToken cancellationToken = default
    ) where T : class
    {
        if (values.Any() is false) return;

        await table.AddRangeAsync(values, cancellationToken);
    }

    // MUST BE TESTED
    public static void SafeRemoveRange<T>(
        this DbSet<T> table, IEnumerable<T> values
    ) where T : class
    {
        foreach (var value in values)
        {
            var entry = table.Entry(value);
            if (entry.State == EntityState.Detached)
            {
                table.Attach(value);
            }
            entry.State = EntityState.Deleted;
        }
    }
}