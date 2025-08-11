using api.Data.Context;
using api.Data.Repositories.Interfaces;
using api.Models.Data;
using api.Models.Requests;
using api.Models.Responses;
using Microsoft.EntityFrameworkCore;
using api.Models.Static;

namespace api.Data.Repositories.Implementations
{
  /// <summary>
  /// Repository for managing tasks.
  /// </summary>
  public class TaskRepository : EfRepository<TaskModel>, ITaskRepository
  {
    private readonly TaskContext _context;
    public TaskRepository(TaskContext context) : base(context)
    {
      _context = context;
    }

    /// <summary>
    /// Retrieves all tasks asynchronously.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A list of all tasks.</returns>
    public async Task<IEnumerable<TaskModel>> GetAllTasksAsync(CancellationToken cancellationToken = default)
    {
      var tasks = await GetAllAsync(cancellationToken);
      return tasks;
    }

    /// <summary>
    /// Retrieves a task by its ID asynchronously.
    /// </summary>
    /// <param name="taskId">The ID of the task to retrieve.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The requested task.</returns>
    public async Task<TaskModel> GetTaskByIdAsync(Guid taskId, CancellationToken cancellationToken = default)
    {
      var task = await FindOneAsync(t => t.Id == taskId, t => t._List, cancellationToken);
      return task;
    }

    /// <summary>
    /// Retrieves all tasks for a specific list asynchronously.
    /// </summary>
    /// <param name="listId">The ID of the list to retrieve tasks from.</param>
    /// <param name="sortPagination">Sorting and pagination information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tasks for the specified list.</returns>
    public async Task<PagedList<TaskModel>> GetTasksByListIdAsync(Guid listId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.ListId == listId & t.Status != (int)TaskStatusEnum.Completed, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    /// <summary>
    /// Retrieves all tasks with a specific status asynchronously.
    /// </summary>
    /// <param name="statusId">The ID of the status to filter tasks by.</param>
    /// <param name="sortPagination">Sorting and pagination information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tasks with the specified status.</returns>
    public async Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Status == statusId, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    /// <summary>
    /// Retrieves all tasks with a specific status asynchronously.
    /// </summary>
    /// <param name="statusId">The ID of the status to filter tasks by.</param>
    /// <param name="sortPagination">Sorting and pagination information.</param>
    /// <param name="user">The user to filter tasks by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tasks with the specified status.</returns>
    public async Task<PagedList<TaskModel>> GetTasksByStatusAsync(int statusId, SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Status == statusId && t.CreatedBy == $"{user.Name};{user.Email};{user.Id}", t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    /// <summary>
    /// Retrieves all tasks with a specific tag asynchronously.
    /// </summary>
    /// <param name="tagId">The ID of the tag to filter tasks by.</param>
    /// <param name="sortPagination">Sorting and pagination information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tasks with the specified tag.</returns>
    public async Task<PagedList<TaskModel>> GetTasksByTagAsync(Guid tagId, SortPagination sortPagination, CancellationToken cancellationToken = default)
    {
      var tasks = await FindManyPaginatedAsync(t => t.Tags.Any(tag => tag.Id == tagId), t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    /// <summary>
    /// Retrieves all tasks for a specific user asynchronously.
    /// </summary>
    /// <param name="user">The user to filter tasks by.</param>
    /// <param name="searchTerm">An optional search term to filter tasks by title or description.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tasks for the specified user.</returns>
    public async Task<IEnumerable<TaskModel>> GetUserTasks(UserModel user, string? searchTerm = null, CancellationToken cancellationToken = default)
    {
      var userId = $"{user.Name};{user.Email};{user.Id}";
      var tasksQuery = _context.Tasks.Include(t => t._List).Where(t => t.CreatedBy == userId || t._List.IsPublic).Where(t => t.Status != (int)TaskStatusEnum.Completed).AsEnumerable();
      if (!string.IsNullOrEmpty(searchTerm))
      {
        tasksQuery = tasksQuery.Where(t => t.Title.Contains(searchTerm) || t.Description.Contains(searchTerm));
      }
      if (tasksQuery == null)
      {
        return Enumerable.Empty<TaskModel>();
      }
      return tasksQuery.ToList();
    }

    /// <summary>
    /// Creates a new task asynchronously.
    /// </summary>
    /// <param name="entity">The task to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task<TaskModel> CreateTaskAsync(TaskModel entity, CancellationToken cancellationToken = default)
    {
      return await AddAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Updates an existing task asynchronously.
    /// </summary>
    /// <param name="entity">The task to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<TaskModel> UpdateTaskAsync(TaskModel entity, CancellationToken cancellationToken = default)
    {
      return UpdateAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Deletes a task asynchronously.
    /// </summary>
    /// <param name="entity">The task to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public Task<bool> DeleteTaskAsync(TaskModel entity, CancellationToken cancellationToken = default)
    {
      return DeleteAsync(entity, cancellationToken);
    }

    /// <summary>
    /// Retrieves today's tasks for a specific user asynchronously.
    /// </summary>
    /// <param name="sortPagination">Sorting and pagination information.</param>
    /// <param name="user">The user to filter tasks by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of today's tasks for the specified user.</returns>
    public async Task<PagedList<TaskModel>> GetTodayTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var today = DateTime.UtcNow.Date;
      var userId = $"{user.Name};{user.Email};{user.Id}";
      var tasks = await FindManyPaginatedAsync(t => t.DueDate.Value.Date == today && t.CreatedBy == userId && t.Status != (int)TaskStatusEnum.Completed, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }

    /// <summary>
    /// Retrieves tomorrow's tasks for a specific user asynchronously.
    /// </summary>
    /// <param name="sortPagination">Sorting and pagination information.</param>
    /// <param name="user">The user to filter tasks by.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged list of tomorrow's tasks for the specified user.</returns>
    public async Task<PagedList<TaskModel>> GetUpcomingTasksAsync(SortPagination sortPagination, UserModel user, CancellationToken cancellationToken = default)
    {
      var today = DateTime.UtcNow.Date;
      var userId = $"{user.Name};{user.Email};{user.Id}";
      var tasks = await FindManyPaginatedAsync(t => t.DueDate.Value.Date > today && t.CreatedBy == userId && t.Status != (int)TaskStatusEnum.Completed, t => t._List, sortPagination, cancellationToken);
      return tasks;
    }
  }
}