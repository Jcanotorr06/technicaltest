using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Services.Interfaces;

namespace api.Services.Implementations
{
  public class TaskService : ITaskService
  {
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
      _taskRepository = taskRepository;
    }

    public async Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
      if (taskId == Guid.Empty)
      {
        throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));
      }

      var response = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
      if (response == null)
      {
        throw new KeyNotFoundException($"Task with ID {taskId} not found.");
      }

      return response;
    }
  }
}