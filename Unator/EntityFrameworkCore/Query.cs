using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Unator.EntityFrameworkCore;

/// <summary>
/// Make queries smaller with ConfigureAwait(false) for Entity Framework Core.
/// </summary>
public static class UQueryExtension
{
    /// <summary>
    /// Find first or default that matches condition.
    /// </summary>
    /// <typeparam name="T">Type of database table.</typeparam>
    /// <returns>Entity if found one, default if not</returns>
    public static async Task<T?> QueryOne<T>(
      this IQueryable<T> query,
      Expression<Func<T, bool>> condition
    )
    {
        return await query.FirstOrDefaultAsync(condition).ConfigureAwait(false);
    }

    /// <summary>
    /// Find first or rdefault that matches all conditions in list.
    /// </summary>
    /// <typeparam name="T">Type of database table.</typeparam>
    /// <returns>Entity if found one, default if not</returns>
    public static async Task<T?> QueryOne<T>(
      this IQueryable<T> query,
      List<Expression<Func<T, bool>>> conditions
    )
    {
        foreach (var condition in conditions) query = query.Where(condition);
        return await query.FirstOrDefaultAsync().ConfigureAwait(false);
    }

    public static async Task<T[]> QueryMany<T>(
      this IQueryable<T> query
    ) => await query.ToArrayAsync().ConfigureAwait(false);

    public static async Task<T[]> QueryMany<T>(
      this IQueryable<T> query,
      Expression<Func<T, bool>> condition
    ) => await query.Where(condition).ToArrayAsync().ConfigureAwait(false);

    public static async Task<T[]> QueryMany<T>(
      this IQueryable<T> query,
      List<Expression<Func<T, bool>>> conditions
    )
    {
        foreach (var condition in conditions) query = query.Where(condition);
        return await query.ToArrayAsync().ConfigureAwait(false);
    }

    public static async Task<bool> Exist<T>(
        this IQueryable<T> query,
        Expression<Func<T, bool>> condition
    ) => await query.AnyAsync(condition).ConfigureAwait(false);
}