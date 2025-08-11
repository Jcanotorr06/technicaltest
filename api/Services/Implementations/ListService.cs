using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Dto.List;
using api.Services.Interfaces;

namespace api.Services.Implementations
{
  /// <summary>
  /// Service for managing task lists.
  /// </summary>
  public class ListService : IListService
  {
    private readonly IListRepository _listRepository;

    public ListService(IListRepository listRepository)
    {
      _listRepository = listRepository;
    }

    /// <summary>
    /// Creates a new task list.
    /// </summary>
    /// <param name="createListDto">Data Transfer Object containing the details of the list to be created.</param>
    /// <param name="user">The user creating the list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a value of the created list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when createListDto is null.</exception>
    public async Task<ReadListDto> CreateListAsync(CreateListDto createListDto, UserModel user, CancellationToken cancellationToken = default)
    {
      if (createListDto == null)
      {
        throw new ArgumentNullException(nameof(createListDto), "CreateListDto cannot be null");
      }
      var listModel = new ListModel
      {
        Id = Guid.NewGuid(),
        Name = createListDto.Name,
        IsPublic = createListDto.IsPublic,
        CreatedBy = $"{user.Name};{user.Email};{user.Id}"
      };
      var createdList = await _listRepository.CreateListAsync(listModel, cancellationToken);
      return new ReadListDto(createdList);
    }

    /// <summary>
    /// Retrieves a task list by its ID.
    /// </summary>
    /// <param name="listId">The ID of the list to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested task list.</returns>
    /// <exception cref="ArgumentException">Thrown when listId is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the list is not found.</exception>
    public async Task<bool> DeleteListAsync(Guid listId, CancellationToken cancellationToken = default)
    {
      if (listId == Guid.Empty)
      {
        throw new ArgumentException("Invalid list ID", nameof(listId));
      }
      var existingList = await _listRepository.GetListByIdAsync(listId, cancellationToken);
      if (existingList == null)
      {
        throw new KeyNotFoundException($"List with ID {listId} not found");
      }
      return await _listRepository.DeleteListAsync(existingList, cancellationToken);
    }

    /// <summary>
    /// Retrieves all task lists.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation, with a value of the list of all task lists.</returns>
    public async Task<IEnumerable<ReadListDto>> GetAllListsAsync(CancellationToken cancellationToken = default)
    {
      var lists = await _listRepository.GetAllListsAsync(cancellationToken);
      return lists.Select(list => new ReadListDto(list));
    }

    public async Task<IEnumerable<ReadListDto>> GetPublicListsAsync(CancellationToken cancellationToken = default)
    {
      var lists = await _listRepository.GetPublicListsAsync(cancellationToken);
      return lists.Select(list => new ReadListDto(list));
    }

    /// <summary>
    /// Retrieves a task list by its ID.
    /// </summary>
    /// <param name="listId">The ID of the list to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested task list.</returns>
    /// <exception cref="ArgumentException">Thrown when listId is invalid.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the list is not found.</exception>
    public async Task<ReadListDto> GetListByIdAsync(Guid listId, UserModel user, CancellationToken cancellationToken = default)
    {
      if (listId == Guid.Empty)
      {
        throw new ArgumentException("Invalid list ID", nameof(listId));
      }
      var list = await _listRepository.GetListByIdAsync(listId, cancellationToken);
      if (list == null)
      {
        throw new KeyNotFoundException($"List with ID {listId} not found");
      }

      if (!list.IsPublic && list.CreatedBy != $"{user.Name};{user.Email};{user.Id}")
      {
        throw new UnauthorizedAccessException("You are not allowed to view this list.");
      }

      return new ReadListDto(list);
    }

    /// <summary>
    /// Retrieves a task list by its ID.
    /// </summary>
    /// <param name="userId">The ID of the user who owns the list.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested task list.</returns>
    /// <exception cref="ArgumentException">Thrown when userId is invalid.</exception>
    public async Task<IEnumerable<ReadListDto>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrWhiteSpace(userId))
      {
        throw new ArgumentException("Invalid user ID", nameof(userId));
      }
      var lists = await _listRepository.GetUserListsAsync(userId, cancellationToken);
      return lists.Select(list => new ReadListDto(list));
    }

    /// <summary>
    /// Updates a task list.
    /// </summary>
    /// <param name="updateListDto">The updated list data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated task list.</returns>
    /// <exception cref="ArgumentNullException">Thrown when updateListDto is null.</exception>
    /// <exception cref="KeyNotFoundException">Thrown when the list is not found.</exception>
    public async Task<ReadListDto> UpdateListAsync(UpdateListDto updateListDto, UserModel user, CancellationToken cancellationToken = default)
    {
      if (updateListDto == null)
      {
        throw new ArgumentNullException(nameof(updateListDto), "UpdateListDto cannot be null");
      }
      var existingList = await _listRepository.GetListByIdAsync(updateListDto.Id, cancellationToken);
      if (existingList == null)
      {
        throw new KeyNotFoundException($"List with ID {updateListDto.Id} not found");
      }

      if (existingList.CreatedBy != $"{user.Name};{user.Email};{user.Id}")
      {
        throw new UnauthorizedAccessException("You are not allowed to update this list.");
      }

      existingList.Name = updateListDto.Name;
      existingList.IsPublic = updateListDto.IsPublic;
      var updatedList = await _listRepository.UpdateListAsync(existingList, cancellationToken);
      return new ReadListDto(updatedList);
    }
  }
}