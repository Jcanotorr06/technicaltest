using api.Models.Dto.List;

namespace api.Services.Interfaces
{
  public interface IListService
  {
    Task<ReadListDto> GetListByIdAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadListDto>> GetAllListsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadListDto>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default);
    Task<ReadListDto> CreateListAsync(CreateListDto createListDto, CancellationToken cancellationToken = default);
    Task<ReadListDto> UpdateListAsync(UpdateListDto updateListDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteListAsync(Guid listId, CancellationToken cancellationToken = default);
  }
}