using api.Models.Dto.Task;
using api.Models.Requests;
using api.Models.Responses;

namespace api.Services.Interfaces
{
  public interface ITaskService
  {
    Task<ReadTaskDto> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadTaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default);
  }
}