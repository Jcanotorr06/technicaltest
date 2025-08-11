using System.IdentityModel.Tokens.Jwt;
using api.Models.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace api.Helpers
{
  public class JWTHelper
  {
    /// <summary>
    /// Extracts user information from a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <returns>The user information extracted from the token.</returns>
    /// <exception cref="Exception">Thrown when the token is invalid.</exception>
    public static UserModel GetUserFromToken(string token)
    {
      var isLocal = Environment.GetEnvironmentVariable("Environment") == "Local";
      if (isLocal)
      {
        // In a local environment, we can bypass token validation
        return new UserModel();
      }

      if (string.IsNullOrEmpty(token))
      {
        throw new ArgumentException("Token is null or empty", nameof(token));
      }
      var handler = new JwtSecurityTokenHandler();
      var jwtToken = handler.ReadJwtToken(token);
      var claims = jwtToken.Claims;

      if (claims == null)
      {
        throw new Exception("Invalid token: No claims found.");
      }

      var userIdClaim = claims.FirstOrDefault(c => c.Type == "oid");
      var userNameClaim = claims.FirstOrDefault(c => c.Type == "name");
      var userEmailClaim = claims.FirstOrDefault(c => c.Type == "preferred_username");

      if (userIdClaim == null || userNameClaim == null || userEmailClaim == null)
      {
        throw new Exception("Invalid token: Missing claims.");
      }

      if (!Guid.TryParse(userIdClaim.Value, out var userId))
      {
        throw new Exception("Invalid token: Invalid user ID.");
      }

      return new UserModel
      {
        Id = userId,
        Name = userNameClaim.Value,
        Email = userEmailClaim.Value
      };
    }

    /// <summary>
    /// Validates a JWT token.
    /// </summary>
    /// <param name="token">The JWT token.</param>
    /// <param name="_logger">The logger instance.</param>
    /// <returns>True if the token is valid; otherwise, false.</returns>
    /// <exception cref="SecurityTokenValidationException">Thrown when the token is invalid.</exception>
    public static async Task<bool> ValidateToken(string token, ILogger _logger)
    {
      var isLocal = Environment.GetEnvironmentVariable("Environment") == "Local";
      if (isLocal)
      {
        _logger.LogWarning("Token validation skipped in local environment.");
        return true;
      }
      if (string.IsNullOrEmpty(token))
      {
        _logger.LogWarning("Token is null or empty.");
        return false;
      }
      var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
        Environment.GetEnvironmentVariable("OpenIdConfiguration"),
        new OpenIdConnectConfigurationRetriever(),
        new HttpDocumentRetriever()
      );

      var discoveryDocument = await configurationManager.GetConfigurationAsync();
      var signinKeys = discoveryDocument.SigningKeys;

      var validationParameters = new TokenValidationParameters
      {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = Environment.GetEnvironmentVariable("TokenIssuer"),
        ValidAudience = Environment.GetEnvironmentVariable("TokenAudience"),
        IssuerSigningKeys = signinKeys
      };

      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
        return validatedToken != null;
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Token validation failed.");
        return false;
      }
    }
  }
}