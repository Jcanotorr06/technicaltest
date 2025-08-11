using api.Data.Context;
using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Requests;
using api.Models.Responses;
using Models.Static;

namespace api.Data.Repositories.Implementations
{
  public class TaskRepository : EfRepository<TaskModel>, ITaskRepository
  {
    public TaskRepository(TaskContext context) : base(context)
    { }

    public async Task<IEnumerable<TaskModel>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
      var tasks = await GetAllAsync(cancellationToken);
      return tasks;
    }

    public async Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
      var task = await FindOneAsync(t => t.Id == taskId, t => t._List, cancellationToken);
      return task;
    }

    public async Task<PagedList<TaskModel>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.ListId == listId & t.Status != (int)TaskStatusEnum.Completed, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    public async Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Status == statusId, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    public async Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Status == statusId && t.CreatedBy == $"{user.Name};{user.Email};{user.Id}", t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    public async Task<PagedList<TaskModel>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Tags.Any(tag => tag.Id == tagId), t => t._List, sortPagination, cancellationToken);
      return tasks;
    }


    public async Task<TaskModel> CreateTaskAsync(TaskModel entity, CancellationToken cancellationToken = default)
    {
      return await AddAsync(entity, cancellationToken);
    }

    public Task<TaskModel> UpdateTaskAsync(TaskModel entity, CancellationToken cancellationToken = default)
    {
      return UpdateAsync(entity, cancellationToken);
    }

    public Task<bool> DeleteTaskAsync(TaskModel entity, CancellationToken cancellationToken = default)
    {
      return DeleteAsync(entity, cancellationToken);
    }

    public async Task<PagedList<TaskModel>> GetTodayTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var today = DateTime.UtcNow.Date;
      var userId = $"{user.Name};{user.Email};{user.Id}";
      var tasks = await FindManyPaginatedAsync(t => t.DueDate.Value.Date == today && t.CreatedBy == userId, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    public async Task<PagedList<TaskModel>> GetUpcomingTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var today = DateTime.UtcNow.Date;
      var userId = $"{user.Name};{user.Email};{user.Id}";
      var tasks = await FindManyPaginatedAsync(t => t.DueDate.Value.Date > today && t.CreatedBy == userId, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }
  }
}