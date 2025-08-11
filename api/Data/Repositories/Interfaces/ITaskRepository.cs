using api.Models.Data;
using api.Models.Requests;
using api.Models.Responses;

namespace api.Data.Repositories.Interfaces
{
  public interface ITaskRepository : IEFRepository<TaskModel>
  {
    Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskModel>> GetAllTasksAsync(CancellationToken cancellationToken = default);
    Task<PagedList<TaskModel>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<PagedList<TaskModel>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default);
    Task<PagedList<TaskModel>> GetTodayTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<PagedList<TaskModel>> GetUpcomingTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default);
    Task<TaskModel> CreateTaskAsync(TaskModel entity, CancellationToken cancellationToken = default);
    Task<TaskModel> UpdateTaskAsync(TaskModel entity, CancellationToken cancellationToken = default);
    Task<bool> DeleteTaskAsync(TaskModel entity, CancellationToken cancellationToken = default);
    Task<IEnumerable<TaskModel>> GetUserTasks(UserModel user, string? searchTerm = null, CancellationToken cancellationToken = default);
  }
}