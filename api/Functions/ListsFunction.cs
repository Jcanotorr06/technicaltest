using System.Net;
using api.Models.Dto.List;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;

namespace api.Functions
{
  public class ListFunction : BaseFunction
  {
    private readonly ILogger<ListFunction> _logger;
    private readonly IListService _listService;

    public ListFunction(ILogger<ListFunction> logger, IListService listService)
    {
      _logger = logger;
      _listService = listService;
    }

    /// <summary>
    /// Retrieves a list by its ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="listId">The ID of the list to retrieve.</param>
    /// <returns>An action result containing the list details or an error response.</returns>
    [Function("GetListById")]
    [OpenApiOperation(operationId: "GetListById", tags: new string[] { "Lists" }, Summary = "Retrieves a list by its ID.", Description = "Retrieves a list by its ID.")]
    [OpenApiParameter(name: "listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the list to retrieve.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReadListDto), Description = "The list details.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "List not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetListById(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "lists/{listId}")] HttpRequest req,
      FunctionContext context,
      Guid listId
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to get list by ID.");

          var list = await _listService.GetListByIdAsync(listId);
          if (list == null)
          {
            return new NotFoundObjectResult($"List with ID {listId} not found.");
          }
          return new OkObjectResult(list);
        },
        context.InvocationId,
        _logger
      );
    }

    /// <summary>
    /// Retrieves all lists.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the list of all lists or an error response.</returns>
    [Function("GetAllLists")]
    [OpenApiOperation(operationId: "GetAllLists", tags: new string[] { "Lists" }, Summary = "Retrieves all lists.", Description = "Retrieves all lists.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ReadListDto>), Description = "A collection of all lists.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetAllLists(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "lists")] HttpRequest req,
      FunctionContext context
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to get all lists.");

          var lists = await _listService.GetAllListsAsync();
          return new OkObjectResult(lists);
        },
        context.InvocationId,
        _logger
      );
    }

    /// <summary>
    /// Retrieves all lists created by a specific user.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the lists created by the user or an error response.</returns>
    [Function("GetUserLists")]
    [OpenApiOperation(operationId: "GetUserLists", tags: new string[] { "Lists" }, Summary = "Retrieves all lists created by a specific user.", Description = "Retrieves all lists created by a specific user.")]
    [OpenApiParameter(name: "userId", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The ID of the user whose lists to retrieve.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(IEnumerable<ReadListDto>), Description = "A collection of lists created by the specified user.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> GetUserLists(
      [HttpTrigger(AuthorizationLevel.Function, "get", Route = "lists/user")] HttpRequest req,
      FunctionContext context
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to get user lists.");

          var userId = req.Query["userId"];
          if (string.IsNullOrEmpty(userId))
          {
            return new BadRequestObjectResult("User ID is required.");
          }

          var lists = await _listService.GetUserListsAsync(userId.ToString());
          return new OkObjectResult(lists);
        },
        context.InvocationId,
        _logger
      );
    }

    /// <summary>
    /// Creates a new list.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the created list details or an error response.</returns>
    [Function("CreateList")]
    [OpenApiOperation(operationId: "CreateList", tags: new string[] { "Lists" }, Summary = "Creates a new list.", Description = "Creates a new list.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(CreateListDto), Required = true, Description = "The details of the list to create.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.Created, contentType: "application/json", bodyType: typeof(ReadListDto), Description = "The created list details.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid input.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> CreateList(
      [HttpTrigger(AuthorizationLevel.Function, "post", Route = "lists")] HttpRequest req,
      FunctionContext context
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to create a list.");

          var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
          var createListDto = JsonConvert.DeserializeObject<CreateListDto>(requestBody);

          if (createListDto == null)
          {
            return new BadRequestObjectResult("Invalid input.");
          }

          var createdList = await _listService.CreateListAsync(createListDto);
          return new ObjectResult(createdList) { StatusCode = StatusCodes.Status201Created };
        },
        context.InvocationId,
        _logger
      );
    }

    /// <summary>
    /// Updates an existing list.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <returns>An action result containing the updated list details or an error response.</returns>
    [Function("UpdateList")]
    [OpenApiOperation(operationId: "UpdateList", tags: new string[] { "Lists" }, Summary = "Updates an existing list.", Description = "Updates an existing list.")]
    [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(UpdateListDto), Required = true, Description = "The details of the list to update.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ReadListDto), Description = "The updated list details.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "List not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.BadRequest, contentType: "application/json", bodyType: typeof(string), Description = "Invalid input.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> UpdateList(
      [HttpTrigger(AuthorizationLevel.Function, "put", Route = "lists")] HttpRequest req,
      FunctionContext context
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to update a list.");

          var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
          var updateListDto = JsonConvert.DeserializeObject<UpdateListDto>(requestBody);

          if (updateListDto == null)
          {
            return new BadRequestObjectResult("Invalid input.");
          }

          var updatedList = await _listService.UpdateListAsync(updateListDto);
          return new OkObjectResult(updatedList);
        },
        context.InvocationId,
        _logger
      );
    }

    /// <summary>
    /// Deletes a list by its ID.
    /// </summary>
    /// <param name="req">The HTTP request.</param>
    /// <param name="context">The function context.</param>
    /// <param name="listId">The ID of the list to delete.</param>
    /// <returns>An action result indicating the outcome of the delete operation.</returns>
    [Function("DeleteList")]
    [OpenApiOperation(operationId: "DeleteList", tags: new string[] { "Lists" }, Summary = "Deletes a list by its ID.", Description = "Deletes a list by its ID.")]
    [OpenApiParameter(name: "listId", In = ParameterLocation.Path, Required = true, Type = typeof(Guid), Description = "The ID of the list to delete.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NoContent, contentType: "application/json", bodyType: typeof(void), Description = "List deleted successfully.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.NotFound, contentType: "application/json", bodyType: typeof(string), Description = "List not found.")]
    [OpenApiResponseWithBody(statusCode: HttpStatusCode.InternalServerError, contentType: "application/json", bodyType: typeof(string), Description = "An error occurred.")]
    public async Task<IActionResult> DeleteList(
      [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "lists/{listId}")] HttpRequest req,
      FunctionContext context,
      Guid listId
    )
    {
      return await TryExecute(
        async () =>
        {
          _logger.LogInformation("C# HTTP trigger function processed a request to delete a list.");

          var result = await _listService.DeleteListAsync(listId);
          if (result)
          {
            return new NoContentResult();
          }
          return new NotFoundObjectResult("List not found.");
        },
        context.InvocationId,
        _logger
      );
    }
  }
}