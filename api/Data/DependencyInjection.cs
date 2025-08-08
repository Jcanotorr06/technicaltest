using api.Data.Context;
using api.Data.Repositories.Implementations;
using api.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace api.Data
{
  public static class DependencyInjection
  {
    public static IServiceCollection AddEntityFrameworkInfrastructureAsync(this IServiceCollection services, IConfiguration configuration)
    {
      var sqlConnectionString = Environment.GetEnvironmentVariable("SQLConnectionString");
      if (string.IsNullOrEmpty(sqlConnectionString))
      {
        throw new InvalidOperationException("SQLConnectionString environment variable is not set.");
      }
      services.AddDbContext<TaskContext>(options =>
          options.UseSqlServer(sqlConnectionString)
      );

      services.AddTransient<ITaskRepository, TaskRepository>();

      return services;
    }
  }
}