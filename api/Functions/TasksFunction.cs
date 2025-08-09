using System.Net;
using api.Models.Dto.Task;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

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
    /// Retrieves a task by its ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="taskId">The ID of the task to retrieve.</param>
    /// <returns>>An action result containing the task details or an error response.</returns>
    [Function("GetTaskById")]
    [OpenApiOperation(operationId: "GetTaskById", tags: new string[] { "Tasks" }, Description = "Retrieves a task by its ID.")]
    [OpenApiParameter(name: "taskId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the task to retrieve.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReadTaskDto), Description = "The task details.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "Task not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetTaskById([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks/{taskId}")] HttpRequest req,
      FunctionContext context,
      Guid taskId
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var task = await _taskService.GetTaskByIdAsync(taskId);
          if (task == null)
          {
            return new NotFoundObjectResult($"Task with ID {taskId} not found.");
          }

          return new OkObjectResult(task);
        },
        context.InvocationId,
        _logger
      );
    }

    /// <summary>
    /// Retrieves all tasks.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the list of tasks or an error response.</returns>
    [Function("GetAllTasks")]
    [OpenApiOperation(operationId: "GetAllTasks", tags: new string[] { "Tasks" }, Description = "Retrieves all tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ReadTaskDto>), Description = "List of tasks.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetAllTasks([HttpTrigger(AuthorizationLevel.Function, "get", Route = "tasks")] HttpRequest req,
      FunctionContext context
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          var tasks = await _taskService.GetAllTasksAsync();
          return new OkObjectResult(tasks);
        },
        context.InvocationId,
        _logger
      );
    }
  }
}