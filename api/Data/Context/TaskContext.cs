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

      modelBuilder.Entity<TaskStatusModel>()
        .HasData(
          new TaskStatusModel { Id = 1, StatusName = "Pending" },
          new TaskStatusModel { Id = 2, StatusName = "In Progress" },
          new TaskStatusModel { Id = 3, StatusName = "Completed" }
        );

      modelBuilder.Entity<ListModel>()
        .HasMany(l => l.Tasks)
        .WithOne(t => t._List)
        .HasForeignKey(t => t.ListId)
        .HasPrincipalKey(l => l.Id);

      modelBuilder.Entity<TaskModel>()
        .HasMany(t => t.Tags)
        .WithMany(tag => tag.Tasks);
    }
  }
}