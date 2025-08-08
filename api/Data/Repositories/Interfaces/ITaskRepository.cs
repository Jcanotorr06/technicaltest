using api.Models.Data;

namespace api.Data.Repositories.Interfaces
{
  public interface ITaskRepository : IEFRepository<TaskModel>
  {
    Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
  }
}