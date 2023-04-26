using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Unator.EntityFrameworkCore;

/// <summary>
/// Result of mutation which return data back
/// </summary>
/// <typeparam name="T">Type of database table</typeparam>
/// <param name="Data">Will be null if Error occurred</param>
/// <param name="Error">Will be null if Data is presented</param>
public record MutationResult<T>(T? Data, Exception? Error) where T : class;

/// <summary>
/// Thrown/returned if entity is not found during mutation
/// </summary>
public class EntityNotFoundException : Exception
{ }

/// <summary>
/// Mutation operations for Entity Framework Core.
/// Purpose is to remove Repository pattern.
/// Not dependent on UQuery.
/// </summary>
public static class UMutationExtension
{
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    private static async Task<Exception?> Mutation<T>(
      this DbContext db, Func<DbSet<T>, Task> mutation
    ) where T : class
    {
        try
        {
            var dbSet = db.Set<T>();
            await mutation(dbSet);
            await db.SaveChangesAsync().ConfigureAwait(false);
            return null;
        }
        catch (Exception ex)
        {
            return ex;
        }
    }

    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    private static async Task<MutationResult<T>> ResultMutation<T>(this DbContext db, Func<DbSet<T>, Task<T>> mutation) where T : class
    {
        try
        {
            var dbSet = db.Set<T>();
            var data = await mutation(dbSet);
            await db.SaveChangesAsync().ConfigureAwait(false);
            return new(data, null);
        }
        catch (Exception ex)
        {
            return new MutationResult<T>(null, ex);
        }
    }

    /// <summary>
    /// Create new entity in database.
    /// </summary>
    /// <typeparam name="T">
    /// Type of database table to work with.
    /// </typeparam>
    /// <returns></returns>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<MutationResult<T>> CreateOne<T>(
      this DbContext db,
      T entity
    ) where T : class
    => await db.ResultMutation<T>(async dbSet =>
    {
        var result = await dbSet.AddAsync(entity).ConfigureAwait(false);
        return result.Entity;
    });

    /// <summary>
    /// Create entities in database
    /// </summary>
    /// <typeparam name="T">Type of database table to work with.</typeparam>
    /// <returns>Null if operation is successful. Or exception from list below.</returns>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<Exception?> CreateMany<T>(
      this DbContext db,
      params T[] entities
    ) where T : class
    => await db.Mutation<T>(
      async dbSet => await dbSet.AddRangeAsync(entities).ConfigureAwait(false)
    );

    /// <summary>
    /// Edit one entity in database and return it back.
    /// </summary>
    /// <typeparam name="T">Type of database table to work with.</typeparam>
    /// <param name="condition">Condition to find entity.</param>
    /// <param name="mutation">Synchronous change of entity.</param>
    /// <returns>
    /// Entity after edit in Data property or exception in Error property
    /// </returns>
    /// <exception cref="EntityNotFoundException">
    /// Returned in Error when entity is not found.
    /// </exception>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<MutationResult<T>> EditOne<T>(
      this DbContext db,
      Expression<Func<T, bool>> condition,
      Action<T> mutation
    ) where T : class
    => await db.ResultMutation<T>(async dbSet =>
    {
        var entity = await dbSet.FirstOrDefaultAsync(condition).ConfigureAwait(false);
        if (entity == null) throw new EntityNotFoundException();
        mutation(entity);
        return entity;
    });

    /// <summary>
    /// Edit all entities which match condition with mutation action.
    /// </summary>
    /// <typeparam name="T">Type of database table to work with.</typeparam>
    /// <param name="mutation">Synchronous change of entity.</param>
    /// <returns>
    /// Entit after edit in Data property or exception in Error property
    /// </returns>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<MutationResult<T[]>> EditMany<T>(
      this DbContext db,
      Expression<Func<T, bool>> condition,
      Action<T> mutation
    ) where T : class
    {
        try
        {
            var dbSet = db.Set<T>();

            var entities = await dbSet.Where(condition).ToArrayAsync().ConfigureAwait(false);
            foreach (var entity in entities) mutation(entity);

            await db.SaveChangesAsync().ConfigureAwait(false);
            return new(entities, null);
        }
        catch (Exception ex) { return new(null, ex); }
    }

    /// <summary>
    /// Delete first entity which matches condition.
    /// </summary>
    /// <typeparam name="T">Type of database table to work with.</typeparam>
    /// <returns>Null if operation is successful. Or exception.</returns>
    /// <exception cref="EntityNotFoundException"/>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<Exception?> DeleteOne<T>(this DbContext db,
      Expression<Func<T, bool>> condition
    ) where T : class
    => await db.Mutation<T>(async dbSet =>
    {
        var entity = await dbSet.FirstOrDefaultAsync(condition).ConfigureAwait(false) ?? throw new EntityNotFoundException();
        dbSet.Remove(entity);
    });

    /// <summary>
    /// Delete entities which match condition.
    /// </summary>
    /// <typeparam name="T">Type of database table to work with.</typeparam>
    /// <returns>Null if operation is successful. Or exception.</returns>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<Exception?> DeleteMany<T>(
      this DbContext db, Expression<Func<T, bool>> condition
    ) where T : class
    => await db.Mutation<T>(async dbSet =>
    {
        var entities = await dbSet.Where(condition).ToArrayAsync().ConfigureAwait(false);
        dbSet.RemoveRange(entities);
    });

    /// <summary>
    /// Delete entity from db and return it back
    /// </summary>
    /// <returns>
    /// Mutation result where any exception is contained in Error property
    /// </returns>
    /// <exception cref="EntityNotFoundException"/>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<MutationResult<T>> RemoveOne<T>(this DbContext db,
      Expression<Func<T, bool>> condition
    ) where T : class
    => await db.ResultMutation<T>(async dbSet =>
    {
        var entity = await dbSet.FirstOrDefaultAsync(condition).ConfigureAwait(false) ?? throw new EntityNotFoundException();
        dbSet.Remove(entity);
        return entity;
    });

    /// <summary>
    /// Delete entities from db and return it back.
    /// </summary>
    /// <typeparam name="T">Type of database table to work with.</typeparam>
    /// <returns>
    /// Array of entities in Data property. OR exception in Error property.
    /// </returns>
    /// <exception cref="DbUpdateException"/>
    /// <exception cref="DbUpdateConcurrencyException"/>
    /// <exception cref="OperationCanceledException"/>
    public static async Task<MutationResult<T[]>> RemoveMany<T>(this DbContext db, Expression<Func<T, bool>> condition) where T : class
    {
        try
        {
            var dbSet = db.Set<T>();

            var entities = await dbSet.Where(condition).ToArrayAsync().ConfigureAwait(false);
            dbSet.RemoveRange(entities);

            await db.SaveChangesAsync().ConfigureAwait(false);
            return new(entities, null);
        }
        catch (Exception ex)
        {
            return new(null, ex);
        }
    }
}