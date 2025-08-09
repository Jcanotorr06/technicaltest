using api.Data.Context;
using api.Data.Repositories.Interfaces;
using api.Models.Data;

namespace api.Data.Repositories.Implementations
{
  /// <summary>
  /// Repository for managing ListModel entities.
  /// Implements methods to interact with the database for list operations.
  /// Inherits from EfRepository to leverage common Entity Framework functionalities.
  /// </summary>
  public class ListRepository : EfRepository<ListModel>, IListRepository
  {
    public ListRepository(TaskContext context) : base(context)
    { }

    /// <summary>
    /// Retrieves a list by its unique identifier.
    /// </summary>
    /// <param name="listId">The unique identifier of the list.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The list with the specified identifier, or null if not found.</returns>
    public async Task<ListModel> GetListByIdAsync(Guid listId, CancellationToken cancellationToken = default)
    {
      var list = await FindOneAsync(l => l.Id == listId, cancellationToken);
      return list;
    }

    /// <summary>
    /// Retrieves all lists from the database.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of all lists.</returns>
    public async Task<IEnumerable<ListModel>> GetAllListsAsync(CancellationToken cancellationToken = default)
    {
      var lists = await GetAllAsync(cancellationToken);
      return lists;
    }

    /// <summary>
    /// Retrieves all lists created by a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A collection of lists created by the specified user.</returns>
    public async Task<IEnumerable<ListModel>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default)
    {
      var lists = await FindManyAsync(l => l.CreatedBy == userId, cancellationToken: cancellationToken);
      return lists;
    }

    /// <summary>
    /// Creates a new list in the database.
    /// </summary>
    /// <param name="entity">The list entity to create.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The created list entity.</returns>
    public async Task<ListModel> CreateListAsync(ListModel entity, CancellationToken cancellationToken = default)
    {
      return await AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Updates an existing list in the database.
    /// </summary>
    /// <param name="entity">The list entity to update.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>The updated list entity.</returns>
    public Task<ListModel> UpdateListAsync(ListModel entity, CancellationToken cancellationToken = default)
    {
      return UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Deletes a list from the database.
    /// </summary>
    /// <param name="entity">The list entity to delete.</param>
    /// <param name="cancellationToken">Cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>True if the list was deleted successfully; otherwise, false.</returns>
    public Task<bool> DeleteListAsync(ListModel entity, CancellationToken cancellationToken = default)
    {
      return DeleteAsync(entity, cancellationToken);
    }
  }
}