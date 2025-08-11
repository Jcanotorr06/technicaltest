using System.Net;
using api.Extensions;
using api.Helpers;
using api.Models.Dto.Task;
using api.Models.Responses;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace api.Functions
{
  public class TasksFunction : BaseFunction
  {
    private readonly ILogger<TasksFunction> _logger;
    private readonly ITaskService _taskService;

    public TasksFunction(ILogger<TasksFunction> logger, ITaskService taskService)
    {
      _logger = logger;
      _taskService = taskService;
    }

    /// <summary>
    /// Retrieves all tasks.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the list of tasks or an error response.</returns>
    [Function("GetAllTasks")]
    [OpenApiOperation(operationId: "GetAllTasks", tags: new string[] { "Tasks" }, Description = "Retrieves all tasks.", Summary = "Get all tasks")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ReadTaskDto>), Description = "List of tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetAllTasks([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "tasks")] HttpRequest req,
      FunctionContext context
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var tasks = await _taskService.GetAllTasksAsync(req.HttpContext.RequestAborted);
          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Retrieves tasks by their list ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="listId">The ID of the list to retrieve tasks from.</param>
    /// <returns>An action result containing the list of tasks or an error response.</returns>
    [Function("GetTasksByListId")]
    [OpenApiOperation(operationId: "GetTasksByListId", tags: new string[] { "Tasks" }, Description = "Retrieves tasks by list ID.", Summary = "Get tasks by list ID")]
    [OpenApiParameter(name: "listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the list to retrieve tasks from.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The maximum number of tasks to return.")]
    [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of tasks to skip.")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The field to sort by.")]
    [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The sort order (asc or desc).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedList<ReadTaskDto>), Description = "List of tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "List not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetTasksByListId([HttpTrigger(AuthorizationLevel.Function, "get", Route = "list/{listId}/tasks")] HttpRequest req,
      FunctionContext context, Guid listId
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");
          var sortPagination = req.Query.GetSortPagination();
          var user = JWTHelper.GetUserFromToken(token);

          var tasks = await _taskService.GetTasksByListIdAsync(listId, sortPagination, user, req.HttpContext.RequestAborted);
          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Retrieves tasks by their status.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="status">The status of the tasks to retrieve.</param>
    /// <returns>An action result containing the list of tasks or an error response.</returns>
    [Function("GetTasksByStatus")]
    [OpenApiOperation(operationId: "GetTasksByStatus", tags: new string[] { "Tasks" }, Description = "Gets tasks by their status.", Summary = "Get tasks by status")]
    [OpenApiParameter(name: "status", In = ParameterLocation.Path, Required = true, Type = typeof(int), Description = "The status of the tasks to retrieve.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The maximum number of tasks to return.")]
    [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of tasks to skip.")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The field to sort by.")]
    [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The sort order (asc or desc).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedList<ReadTaskDto>), Description = "List of tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "List not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetTasksByStatus([HttpTrigger(AuthorizationLevel.Function, "get", Route = "status/{status}/tasks")] HttpRequest req,
      FunctionContext context,
      int status
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var sortPagination = req.Query.GetSortPagination();

          var tasks = await _taskService.GetTasksByStatusAsync(status, sortPagination, req.HttpContext.RequestAborted);
          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Retrieves tasks by their tag ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="tagId">The ID of the tag to retrieve tasks from.</param>
    /// <returns>An action result containing the list of tasks or an error response.</returns>
    [Function("GetTasksByTag")]
    [OpenApiOperation(operationId: "GetTasksByTag", tags: new string[] { "Tasks" }, Description = "Gets tasks by their tag.", Summary = "Get tasks by tag")]
    [OpenApiParameter(name: "tagId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The tag of the tasks to retrieve.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The maximum number of tasks to return.")]
    [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of tasks to skip.")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The field to sort by.")]
    [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The sort order (asc or desc).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedList<ReadTaskDto>), Description = "List of tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "List not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetTasksByTag([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tags/{tag}/tasks")] HttpRequest req,
      FunctionContext context,
      Guid tagId
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var sortPagination = req.Query.GetSortPagination();

          var tasks = await _taskService.GetTasksByTagAsync(tagId, sortPagination, req.HttpContext.RequestAborted);
          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    [Function("GetTodayTasks")]
    [OpenApiOperation(operationId: "GetTodayTasks", tags: new string[] { "Tasks" }, Description = "Retrieves today's tasks.", Summary = "Get today's tasks")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The maximum number of tasks to return.")]
    [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of tasks to skip.")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The field to sort by.")]
    [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The sort order (asc or desc).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedList<ReadTaskDto>), Description = "List of today's tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "No tasks found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetTodayTasks([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks/today")] HttpRequest req,
      FunctionContext context
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var sortPagination = req.Query.GetSortPagination();
          var user = JWTHelper.GetUserFromToken(token);
          var tasks = await _taskService.GetTodayTasksAsync(sortPagination, user, req.HttpContext.RequestAborted);
          if (tasks == null)
          {
            return new NotFoundObjectResult("No tasks found.");
          }

          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    [Function("GetUpcomingTasks")]
    [OpenApiOperation(operationId: "GetUpcomingTasks", tags: new string[] { "Tasks" }, Description = "Retrieves upcoming tasks.", Summary = "Get upcoming tasks")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The maximum number of tasks to return.")]
    [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of tasks to skip.")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The field to sort by.")]
    [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The sort order (asc or desc).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedList<ReadTaskDto>), Description = "List of upcoming tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "No tasks found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetUpcomingTasks([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks/upcoming")] HttpRequest req,
      FunctionContext context
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var sortPagination = req.Query.GetSortPagination();
          var user = JWTHelper.GetUserFromToken(token);
          var tasks = await _taskService.GetUpcomingTasksAsync(sortPagination, user, req.HttpContext.RequestAborted);
          if (tasks == null)
          {
            return new NotFoundObjectResult("No tasks found.");
          }

          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    [Function("GetCompletedTasks")]
    [OpenApiOperation(operationId: "GetCompletedTasks", tags: new string[] { "Tasks" }, Description = "Retrieves completed tasks.", Summary = "Get completed tasks")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiParameter(name: "limit", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The maximum number of tasks to return.")]
    [OpenApiParameter(name: "offset", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of tasks to skip.")]
    [OpenApiParameter(name: "sortBy", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The field to sort by.")]
    [OpenApiParameter(name: "sortOrder", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The sort order (asc or desc).")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(PagedList<ReadTaskDto>), Description = "List of completed tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "No tasks found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetCompletedTasks([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks/completed")] HttpRequest req,
      FunctionContext context
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var sortPagination = req.Query.GetSortPagination();
          var user = JWTHelper.GetUserFromToken(token);

          var tasks = await _taskService.GetCompletedTasksAsync(sortPagination, user, req.HttpContext.RequestAborted);
          if (tasks == null)
          {
            return new NotFoundObjectResult("No tasks found.");
          }

          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Retrieves a task by its ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="taskId">The ID of the task to retrieve.</param>
    /// <returns>>An action result containing the task details or an error response.</returns>
    [Function("WGetTaskById")]
    [OpenApiOperation(operationId: "WGetTaskById", tags: new string[] { "Tasks" }, Description = "Retrieves a task by its ID.", Summary = "Get task by ID")]
    [OpenApiParameter(name: "taskId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the task to retrieve.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReadTaskDto), Description = "The task details.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "Task not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> WGetTaskById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks/{taskId}")] HttpRequest req,
      FunctionContext context,
      Guid taskId
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var user = JWTHelper.GetUserFromToken(token);
          var task = await _taskService.GetTaskByIdAsync(taskId, user, req.HttpContext.RequestAborted);
          if (task == null)
          {
            return new NotFoundObjectResult($"Task with ID {taskId} not found.");
          }

          return new OkObjectResult(task);
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the created task or an error response.</returns>
    [Function("CreateTask")]
    [OpenApiOperation(operationId: "CreateTask", tags: new string[] { "Tasks" }, Description = "Creates a new task.", Summary = "Create a new task")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateTaskDto), Required = true, Description = "The details of the task to create.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ReadTaskDto), Description = "The created task.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid input.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> CreateTask([HttpTrigger(AuthorizationLevel.Function, "post", Route = "tasks")] HttpRequest req,
      FunctionContext context
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
          var createTaskDto = JsonConvert.DeserializeObject<CreateTaskDto>(requestBody);
          var user = JWTHelper.GetUserFromToken(token);

          if (createTaskDto == null)
          {
            return new BadRequestObjectResult("Invalid input.");
          }

          var createdTask = await _taskService.CreateTaskAsync(createTaskDto, user, req.HttpContext.RequestAborted);
          return new ObjectResult(createdTask) { StatusCode = StatusCodes.Status201Created };
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Updates an existing task.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the updated task or an error response.</returns>
    [Function("UpdateTask")]
    [OpenApiOperation(operationId: "UpdateTask", tags: new string[] { "Tasks" }, Description = "Updates an existing task.", Summary = "Update an existing task")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateTaskDto), Required = true, Description = "The details of the task to update.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReadTaskDto), Description = "The updated task.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid input.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "Task not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> UpdateTask([HttpTrigger(AuthorizationLevel.Function, "put", Route = "tasks")] HttpRequest req,
      FunctionContext context
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
          var updateTaskDto = JsonConvert.DeserializeObject<UpdateTaskDto>(requestBody);

          if (updateTaskDto == null)
          {
            return new BadRequestObjectResult("Invalid input.");
          }

          var user = JWTHelper.GetUserFromToken(token);
          var updatedTask = await _taskService.UpdateTaskAsync(updateTaskDto, user, req.HttpContext.RequestAborted);
          return new ObjectResult(updatedTask) { StatusCode = StatusCodes.Status200OK };
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Retrieves a task by its ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="taskId">The ID of the task to retrieve.</param>
    /// <returns>An action result containing the task or an error response.</returns>
    [Function("CompleteTask")]
    [OpenApiOperation(operationId: "CompleteTask", tags: new string[] { "Tasks" }, Description = "Marks a task as completed.", Summary = "Complete a task")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReadTaskDto), Description = "The completed task.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid input.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "Task not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> CompleteTask([HttpTrigger(AuthorizationLevel.Function, "post", Route = "tasks/{taskId}/complete")] HttpRequest req,
      FunctionContext context,
      Guid taskId
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to complete a task.");

          var user = JWTHelper.GetUserFromToken(token);
          var completedTask = await _taskService.CompleteTaskAsync(taskId, user, req.HttpContext.RequestAborted);
          return new ObjectResult(completedTask) { StatusCode = StatusCodes.Status200OK };
        },
        context.InvocationId,
        _logger,
        token
      );
    }

    /// <summary>
    /// Retrieves a task by its ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="taskId">The ID of the task to retrieve.</param>
    /// <returns>An action result containing the task or an error response.</returns>
    [Function("DeleteTask")]
    [OpenApiOperation(operationId: "DeleteTask", tags: new string[] { "Tasks" }, Description = "Deletes a task.", Summary = "Delete a task")]
    [OpenApiParameter(name: "listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the list to delete.")]
    [OpenApiParameter(name: "Authorization", In = ParameterLocation.Header, Required = true, Type = typeof(string), Description = "Bearer token for authorization.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(string), Description = "Task deleted successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "Task not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> DeleteTask([HttpTrigger(AuthorizationLevel.Function, "delete", Route = "tasks/{taskId}")] HttpRequest req,
      FunctionContext context,
      Guid taskId
    )
    {
      var token = req.Headers[HeaderNames.Authorization].ToString().Replace("Bearer ", "");
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to delete a task.");

          var user = JWTHelper.GetUserFromToken(token);
          var result = await _taskService.DeleteTaskAsync(taskId, user, req.HttpContext.RequestAborted);
          if (!result)
          {
            return new NotFoundObjectResult($"Task with ID {taskId} not found.");
          }

          return new NoContentResult();
        },
        context.InvocationId,
        _logger,
        token
      );
    }
  }
}