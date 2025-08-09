using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Dto.Task;
using api.Models.Requests;
using api.Models.Responses;
using api.Services.Interfaces;
using Azure;

namespace api.Services.Implementations
{
  public class TaskService : ITaskService
  {
    private readonly ITaskRepository _taskRepository;

    public TaskService(ITaskRepository taskRepository)
    {
      _taskRepository = taskRepository;
    }

    public async Task<IEnumerable<ReadTaskDto>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
      var response = new List<ReadTaskDto>();
      var tasks = await _taskRepository.GetAllTasksAsync(cancellationToken);
      foreach (var task in tasks)
      {
        response.Add(new ReadTaskDto(task));
      }
      return response;
    }

    public async Task<ReadTaskDto> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
      if (taskId == Guid.Empty)
      {
        throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));
      }

      var task = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
      if (task == null)
      {
        throw new KeyNotFoundException($"Task with ID {taskId} not found.");
      }
      var response = new ReadTaskDto(task);
      return response;
    }

    public async Task<PagedList<ReadTaskDto>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      if (listId == Guid.Empty)
      {
        throw new ArgumentException("List ID cannot be empty.", nameof(listId));
      }

      var tasks = await _taskRepository.GetTasksByListIdAsync(listId, sortPagination, cancellationToken);
      var response = new PagedList<ReadTaskDto>(
        tasks.Items.Select(task => new ReadTaskDto(task)),
        tasks.TotalCount,
        tasks.CurrentPage,
        tasks.PageSize
      );
      return response;
    }

    public async Task<PagedList<ReadTaskDto>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      if (statusId <= 0)
      {
        throw new ArgumentException("Status ID must be positive.", nameof(statusId));
      }

      var tasks = await _taskRepository.GetTasksByStatusAsync(statusId, sortPagination, cancellationToken);
      var response = new PagedList<ReadTaskDto>(
        tasks.Items.Select(task => new ReadTaskDto(task)),
        tasks.TotalCount,
        tasks.CurrentPage,
        tasks.PageSize
      );
      return response;
    }

    public async Task<PagedList<ReadTaskDto>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      if (tagId == Guid.Empty)
      {
        throw new ArgumentException("Tag ID cannot be empty.", nameof(tagId));
      }

      var tasks = await _taskRepository.GetTasksByTagAsync(tagId, sortPagination, cancellationToken);
      var response = new PagedList<ReadTaskDto>(
        tasks.Items.Select(task => new ReadTaskDto(task)),
        tasks.TotalCount,
        tasks.CurrentPage,
        tasks.PageSize
      );
      return response;
    }
  }
}