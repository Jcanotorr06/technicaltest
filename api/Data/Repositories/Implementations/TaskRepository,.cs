using api.Data.Context;
using api.Data.Repositories.Interfaces;
using api.Models.Data;

namespace api.Data.Repositories.Implementations
{
  public class TaskRepository : EfRepository<TaskModel>, ITaskRepository
  {
    public TaskRepository(TaskContext context) : base(context)
    { }

    public async Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
      var task = await GetByIdAsync(taskId, cancellationToken);
      return task;
    }
  }
}