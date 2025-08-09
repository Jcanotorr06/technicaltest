using api.Models.Data;

namespace api.Data.Repositories.Interfaces
{
  public interface IListRepository : IEFRepository<ListModel>
  {
    Task<ListModel> GetListByIdAsync(Guid listId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ListModel>> GetAllListsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ListModel>> GetUserListsAsync(string userId, CancellationToken cancellationToken = default);
    Task<ListModel> CreateListAsync(ListModel entity, CancellationToken cancellationToken = default);
    Task<ListModel> UpdateListAsync(ListModel entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteListAsync(ListModel entity, CancellationToken cancellationToken = default);
  }
}