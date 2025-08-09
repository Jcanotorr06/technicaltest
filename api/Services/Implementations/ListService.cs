using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Dto.List;
using api.Services.Interfaces;

namespace api.Services.Implementations
{
  public class ListService : IListService
  {
    private readonly IListRepository _listRepository;

    public ListService(IListRepository listRepository)
    {
      _listRepository = listRepository;
    }

    public async Task<ReadListDto> CreateListAsync(CreateListDto createListDto, CancellationToken cancellationToken = default)
    {
      if (createListDto == null)
      {
        throw new ArgumentNullException(nameof(createListDto), "CreateListDto cannot be null");
      }
      var listModel = new ListModel
      {
        Id = Guid.NewGuid(),
        Name = createListDto.Name,
        CreatedBy = "userId", // This should be replaced with actual user ID logic
      };
      var createdList = await _listRepository.CreateListAsync(listModel, cancellationToken);
      return new ReadListDto(createdList);
    }

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

    public async Task<IEnumerable<ReadListDto>> GetAllListsAsync(CancellationToken cancellationToken = default)
    {
      var lists = await _listRepository.GetAllListsAsync(cancellationToken);
      return lists.Select(list => new ReadListDto(list));
    }

    public async Task<ReadListDto> GetListByIdAsync(Guid listId, CancellationToken cancellationToken = default)
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
      return new ReadListDto(list);
    }

    public async Task<IEnumerable<ReadListDto>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrWhiteSpace(userId))
      {
        throw new ArgumentException("Invalid user ID", nameof(userId));
      }
      var lists = await _listRepository.GetUserListsAsync(userId, cancellationToken);
      return lists.Select(list => new ReadListDto(list));
    }

    public async Task<ReadListDto> UpdateListAsync(UpdateListDto updateListDto, CancellationToken cancellationToken = default)
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
      existingList.Name = updateListDto.Name;
      var updatedList = await _listRepository.UpdateListAsync(existingList, cancellationToken);
      return new ReadListDto(updatedList);
    }
  }
}