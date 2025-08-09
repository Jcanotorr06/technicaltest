using api.Data.Context;
using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Requests;
using api.Models.Responses;

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
      var task = await FindOneAsync(t => t.Id == taskId, cancellationToken);
      return task;
    }

    public async Task<PagedList<TaskModel>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.ListId == listId, sortPagination, cancellationToken);
      return tasks;
    }

    public async Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Status == statusId, sortPagination, cancellationToken);
      return tasks;
    }

    public async Task<PagedList<TaskModel>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Tags.Any(tag => tag.Id == tagId), sortPagination, cancellationToken);
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
  }
}