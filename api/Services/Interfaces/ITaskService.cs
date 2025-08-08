using api.Models.Data;

namespace api.Services.Interfaces
{
  public interface ITaskService
  {
    Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default);
  }
}