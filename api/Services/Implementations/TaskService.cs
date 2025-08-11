using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Dto.Task;
using api.Models.Requests;
using api.Models.Responses;
using api.Services.Interfaces;
using api.Models.Static;

namespace api.Services.Implementations
{
  public class TaskService : ITaskService
  {
    private readonly ITaskRepository _taskRepository;
    private readonly IListRepository _listRepository;

    public TaskService(ITaskRepository taskRepository, IListRepository listRepository)
    {
      _taskRepository = taskRepository;
      _listRepository = listRepository;
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

    public async Task<ReadTaskDto> GetTaskByIdAsync(Guid taskId, UserModel user, CancellationToken cancellationToken = default)
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

      if (!task._List.IsPublic && task._List.CreatedBy != $"{user.Name};{user.Email};{user.Id}")
      {
        throw new UnauthorizedAccessException("You do not have permission to access this task.");
      }

      var response = new ReadTaskDto(task);
      return response;
    }

    public async Task<PagedList<ReadTaskDto>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      if (listId == Guid.Empty)
      {
        throw new ArgumentException("List ID cannot be empty.", nameof(listId));
      }

      var list = await _listRepository.GetListByIdAsync(listId, cancellationToken);
      if (list == null)
      {
        throw new KeyNotFoundException($"List with ID {listId} not found.");
      }

      if (!list.IsPublic && list.CreatedBy != $"{user.Name};{user.Email};{user.Id}")
      {
        throw new UnauthorizedAccessException("You do not have permission to access this list.");
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

    public async Task<PagedList<ReadTaskDto>> GetTodayTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var tasks = await _taskRepository.GetTodayTasksAsync(sortPagination, user, cancellationToken);
      var response = new PagedList<ReadTaskDto>(
        tasks.Items.Select(t => new ReadTaskDto(t)),
        tasks.TotalCount,
        tasks.CurrentPage,
        tasks.PageSize
      );
      return response;
    }

    public async Task<PagedList<ReadTaskDto>> GetUpcomingTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var tasks = await _taskRepository.GetUpcomingTasksAsync(sortPagination, user, cancellationToken);
      var response = new PagedList<ReadTaskDto>(
        tasks.Items.Select(t => new ReadTaskDto(t)),
        tasks.TotalCount,
        tasks.CurrentPage,
        tasks.PageSize
      );
      return response;
    }

    public async Task<PagedList<ReadTaskDto>> GetCompletedTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var tasks = await _taskRepository.GetTasksByStatusAsync((int)TaskStatusEnum.Completed, sortPagination, user, cancellationToken);
      var response = new PagedList<ReadTaskDto>(
        tasks.Items.Select(t => new ReadTaskDto(t)),
        tasks.TotalCount,
        tasks.CurrentPage,
        tasks.PageSize
      );
      return response;
    }

    public async Task<ReadTaskDto> CreateTaskAsync(CreateTaskDto createTaskDto, UserModel user, CancellationToken cancellationToken = default)
    {
      if (createTaskDto == null)
      {
        throw new ArgumentNullException(nameof(createTaskDto));
      }

      var task = new TaskModel
      {
        Id = Guid.NewGuid(),
        Title = createTaskDto.Title,
        Description = createTaskDto.Description,
        DueDate = createTaskDto.DueDate,
        Status = createTaskDto.Status,
        ListId = createTaskDto.ListId,
        CreatedBy = $"{user.Name};{user.Email};{user.Id}",
        AssignedTo = $"{user.Name};{user.Email};{user.Id}"
      };

      var createdTask = await _taskRepository.CreateTaskAsync(task, cancellationToken);
      return new ReadTaskDto(createdTask);
    }

    public async Task<ReadTaskDto> UpdateTaskAsync(UpdateTaskDto updateTaskDto, UserModel user, CancellationToken cancellationToken = default)
    {
      if (updateTaskDto == null)
      {
        throw new ArgumentNullException(nameof(updateTaskDto));
      }

      var existingTask = await _taskRepository.GetTaskByIdAsync(updateTaskDto.Id, cancellationToken);
      if (existingTask == null)
      {
        throw new KeyNotFoundException($"Task with ID {updateTaskDto.Id} not found.");
      }

      if (existingTask.AssignedTo != $"{user.Name};{user.Email};{user.Id}" && existingTask.CreatedBy != $"{user.Name};{user.Email};{user.Id}")
      {
        throw new UnauthorizedAccessException("You are not allowed to update this task.");
      }

      if (existingTask.Status == (int)TaskStatusEnum.Completed)
      {
        throw new InvalidOperationException("Cannot update a completed task.");
      }

      existingTask.Title = updateTaskDto.Title;
      existingTask.Description = updateTaskDto.Description;
      existingTask.DueDate = updateTaskDto.DueDate;
      existingTask.Status = updateTaskDto.Status;
      existingTask.ListId = updateTaskDto.ListId;
      existingTask.AssignedTo = updateTaskDto.AssignedTo;
      existingTask.Order = updateTaskDto.Order;

      var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask, cancellationToken);
      return new ReadTaskDto(updatedTask);
    }

    public async Task<bool> DeleteTaskAsync(Guid taskId, UserModel user, CancellationToken cancellationToken = default)
    {
      if (taskId == Guid.Empty)
      {
        throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));
      }

      var existingTask = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
      if (existingTask == null)
      {
        throw new KeyNotFoundException($"Task with ID {taskId} not found.");
      }

      if (existingTask.AssignedTo != $"{user.Name};{user.Email};{user.Id}" && existingTask.CreatedBy != $"{user.Name};{user.Email};{user.Id}")
      {
        throw new UnauthorizedAccessException("You are not allowed to delete this task.");
      }

      if (existingTask.Status == (int)TaskStatusEnum.Completed)
      {
        throw new InvalidOperationException("Cannot delete a completed task.");
      }

      return await _taskRepository.DeleteTaskAsync(existingTask, cancellationToken);
    }

    public async Task<ReadTaskDto> CompleteTaskAsync(Guid taskId, UserModel user, CancellationToken cancellationToken = default)
    {
      if (taskId == Guid.Empty)
      {
        throw new ArgumentException("Task ID cannot be empty.", nameof(taskId));
      }

      var existingTask = await _taskRepository.GetTaskByIdAsync(taskId, cancellationToken);
      if (existingTask == null)
      {
        throw new KeyNotFoundException($"Task with ID {taskId} not found.");
      }

      if (existingTask.Status == (int)TaskStatusEnum.Completed)
      {
        throw new InvalidOperationException("Cannot complete a completed task.");
      }

      var list = await _listRepository.GetListByIdAsync(existingTask.ListId, cancellationToken);
      if (list == null)
      {
        throw new KeyNotFoundException($"List with ID {existingTask.ListId} not found.");
      }

      if (existingTask.AssignedTo != $"{user.Name};{user.Email};{user.Id}" && existingTask.CreatedBy != $"{user.Name};{user.Email};{user.Id}" && !list.IsPublic)
      {
        throw new UnauthorizedAccessException("You are not allowed to complete this task.");
      }

      existingTask.Status = (int)TaskStatusEnum.Completed;
      var updatedTask = await _taskRepository.UpdateTaskAsync(existingTask, cancellationToken);
      return new ReadTaskDto(updatedTask);
    }

    public async Task<IEnumerable<ReadTaskDto>> GetUserTasksAsync(UserModel user, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
      var tasks = await _taskRepository.GetUserTasks(user, searchTerm, cancellationToken);
      return tasks.Select(t => new ReadTaskDto(t));
    }
  }
}