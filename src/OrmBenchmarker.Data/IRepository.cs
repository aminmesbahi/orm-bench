namespace OrmBenchmarker.Data;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

public interface IRepository<T, TKey> where T : class
{
    /// <summary>
    /// Gets an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The entity with the specified identifier.</returns>
    Task<T> GetByIdAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a paginated list of entities.
    /// </summary>
    /// <param name="pageIndex">The zero-based page index.</param>
    /// <param name="pageSize">The number of entities per page.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paginated list of entities.</returns>
    Task<List<T>> GetPagedAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets entities that match the specified filter.
    /// </summary>
    /// <param name="filter">A predicate to filter the entities.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A filtered list of entities.</returns>
    Task<List<T>> GetFilteredAsync(
        Func<T, bool> filter,
        CancellationToken cancellationToken = default
    );

    /// <summary>
    /// Inserts a new entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The inserted entity.</returns>
    Task<T> InsertAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Inserts multiple entities into the repository.
    /// </summary>
    /// <param name="entities">The list of entities to insert.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task InsertRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The updated entity.</returns>
    Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an entity by its identifier.
    /// </summary>
    /// <param name="id">The identifier of the entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteAsync(TKey id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes the specified entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}


