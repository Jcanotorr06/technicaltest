using System.Linq.Expressions;
using api.Data.Context;
using api.Data.Repositories.Interfaces;
using api.Models.Requests;
using api.Models.Responses;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Repositories.Implementations
{
  /// <summary>
  /// Generic Entity Framework repository implementation.
  /// </summary>
  /// <typeparam name="T">The type of the entity.</typeparam>
  public class EfRepository<T> : IEFRepository<T> where T : class
  {
    private readonly TaskContext _context;
    public EfRepository(TaskContext context)
    {
      _context = context;
    }

    /// <summary>
    /// Gets an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity with the specified ID, or null if not found.</returns>
    public async Task<T> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
      var keyValues = new object[] { id };
      return await _context.Set<T>().FindAsync(keyValues, cancellationToken);
    }

    /// <summary>
    /// Finds a single entity that matches the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The entity that matches the predicate, or null if not found.</returns>
    public async Task<T> FindOneAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
      return await _context.Set<T>().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    /// <summary>
    /// Finds multiple entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of entities that match the predicate.</returns>
    public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default)
    {
      return await _context.Set<T>().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Finds multiple entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to filter the entities.</param>
    /// <param name="sortPagination">The pagination and sorting information.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A paged list of entities that match the predicate.</returns>
    public async Task<PagedList<T>> FindManyPaginatedAsync(Expression<Func<T, bool>> predicate, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var query = _context.Set<T>().Where(predicate);

      // Apply sorting
      if (!string.IsNullOrEmpty(sortPagination.SortBy))
      {
        var isDescending = sortPagination.SortOrder?.ToLower() == "desc";
        query = isDescending ? query.OrderByDescending(e => EF.Property<object>(e, sortPagination.SortBy)) : query.OrderBy(e => EF.Property<object>(e, sortPagination.SortBy));
      }

      return await PagedList<T>.CreateAsync(query, sortPagination.Page, sortPagination.Limit, cancellationToken);
    }

    /// <summary>
    /// Gets all entities of type T.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A list of all entities of type T.</returns>
    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
      return await _context.Set<T>().ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Adds a new entity to the context.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The added entity.</returns>
    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
      await _context.Set<T>().AddAsync(entity, cancellationToken);
      await _context.SaveChangesAsync(cancellationToken);
      return entity;
    }

    /// <summary>
    /// Updates an existing entity in the context.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The updated entity.</returns>
    public async Task<T> UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
      _context.Set<T>().Update(entity);
      await _context.SaveChangesAsync(cancellationToken);
      return entity;
    }

    /// <summary>
    /// Deletes an entity from the context.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>True if the entity was deleted; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
      if (entity != null)
      {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
      }
      return false;
    }
  }
}