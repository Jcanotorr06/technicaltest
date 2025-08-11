using System.Net;
using api.Helpers;
using api.Models.Messages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Functions
{
  /// <summary>
  /// BaseFunction class that provices a standardized flow for executing functions with error handling and logging.
  /// Inherit from this class to utilize the TryExecute method for consistent error management across all functions.
  /// </summary>
  public class BaseFunction
  {
    /// <summary>
    /// Executes a given function with standardized error handling and logging.
    /// </summary>
    /// <param name="method">Method to execute</param>
    /// <param name="invocationId">Unique identifier for the invocation</param>
    /// <param name="_logger">Logger instance for logging errors</param>
    /// <returns>Action result indicating success or failure</returns>
    public async Task<IActionResult> TryExecute(
      Func<Task<IActionResult>> method, string invocationId, ILogger _logger, string token = ""
    )
    {
      try
      {
        var validToken = await JWTHelper.ValidateToken(token, _logger);
        if (!validToken)
        {
          return new UnauthorizedResult();
        }
        return await method();
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, $"InvocationId: {invocationId} - An error occurred: {ex.Message}");
        InfoMessage infoMessage = new(ex.ToString(), ex.Message, HttpStatusCode.InternalServerError, invocationId.ToString());
        return new ObjectResult(infoMessage) { StatusCode = (int)HttpStatusCode.InternalServerError };
      }
    }
  }
}