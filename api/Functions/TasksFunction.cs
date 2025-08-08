using System.Net;
using api.Models.Data;
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
    /// A simple HTTP trigger function that returns a greeting message.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="name">The name to greet.</param>
    /// <returns>A greeting message or an error response.</returns>
    [Function("HelloWorld")]
    [OpenApiOperation(operationId: "HelloWorld", tags: new string[] { "Tasks" }, Description = "Returns a greeting message.")]
    [OpenApiParameter(name: "name", In = ParameterLocation.Path, Required = true, Type = typeof(string), Description = "The name to greet.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(string), Description = "The OK response message")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "The Bad Request response message")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "The Internal Server Error response message")]
    public async Task<IActionResult> HelloWorld([HttpTrigger(AuthorizationLevel.Function, "get", Route = "hello/{name}")] HttpRequest req,
      FunctionContext context,
      string name
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request.");

          if (string.IsNullOrWhiteSpace(name))
          {
            return new BadRequestObjectResult("Please pass a name on the query string or in the request body");
          }

          await Task.Delay(10); // Simulate some async work
          return new OkObjectResult($"Hello, {name}!");
        },
        context.InvocationId,
        _logger
      );
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
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(TaskModel), Description = "The task details.")]
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
  }
}