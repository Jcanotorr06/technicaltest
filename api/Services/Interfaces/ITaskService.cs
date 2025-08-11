using api.Models.Data;
using api.Models.Dto.Task;
using api.Models.Requests;
using api.Models.Responses;

namespace api.Services.Interfaces
{
  public interface ITaskService
  {
    Task<ReadTaskDto> GetTaskByIdAsync(Guid taskId, UserModel user, CancellationToken cancellationToken = default);
    Task<IEnumerable<ReadTaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetTodayTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetUpcomingTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<PagedList<ReadTaskDto>> GetCompletedTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<ReadTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, UserModel user, CancellationToken cancellationToken = default);
    Task<ReadTaskDto> UpdateTaskAsync(UpdateTaskDto updateTaskDto, UserModel user, CancellationToken cancellationToken = default);
    Task<ReadTaskDto> CompleteTaskAsync(Guid taskId, UserModel user, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(Guid taskId, UserModel user, CancellationToken cancellationToken = default);
  }
}