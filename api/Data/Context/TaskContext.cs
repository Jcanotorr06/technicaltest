using api.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace api.Data.Context
{
  public class TaskContext : DbContext
  {
    public TaskContext()
    {
      Database.EnsureCreated();
    }

    public TaskContext(DbContextOptions<TaskContext> options) : base(options)
    { }

    public DbSet<TaskStatusModel> TaskStatuses { get; set; }
    public DbSet<TaskModel> Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.Entity<TaskStatusModel>()
        .HasMany(ts => ts.Tasks)
        .WithOne(t => t._Status)
        .HasForeignKey(t => t.Status)
        .HasPrincipalKey(ts => ts.Id);
    }
  }
}