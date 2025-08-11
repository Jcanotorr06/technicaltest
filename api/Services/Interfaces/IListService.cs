using api.Models.Data;
using api.Models.Dto.List;

namespace api.Services.Interfaces
{
  public interface IListService
  {
    Task<ReadListDto> GetListByIdAsync(Guid listId, UserModel user, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadListDto>> GetAllListsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadListDto>> GetPublicListsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadListDto>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default);
    Task<ReadListDto> CreateListAsync(CreateListDto createListDto, UserModel user, CancellationToken cancellationToken = default);
    Task<ReadListDto> UpdateListAsync(UpdateListDto updateListDto, UserModel user, CancellationToken cancellationToken = default);
    Task<bool> DeleteListAsync(Guid listId, CancellationToken cancellationToken = default);
  }
}