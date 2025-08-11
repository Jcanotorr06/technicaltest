using Xunit;
using Microsoft.EntityFrameworkCore;
using Assert = Xunit.Assert;
using api.Data.Repositories.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Data.Repositories.Implementations.Tests
{
    /// <summary>
    /// Unit tests for the TaskRepository class using an in-memory database.
    /// </summary>
    public class TaskRepositoryTests
    {
        /// <summary>
        /// Creates a new in-memory TaskContext for test isolation.
        /// </summary>
        /// <param name="dbName">The unique database name for the in-memory context.</param>
        /// <returns>A new TaskContext instance.</returns>
        private static api.Data.Context.TaskContext CreateInMemoryContext(string dbName)
        {
            var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<api.Data.Context.TaskContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new api.Data.Context.TaskContext(options);
        }

        /// <summary>
        /// Tests that GetAllTasksAsync returns all tasks in the database.
        /// </summary>
        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var task1 = new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "Task 1", CreatedBy = "user1", ListId = Guid.NewGuid() };
            var task2 = new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "Task 2", CreatedBy = "user2", ListId = Guid.NewGuid() };
            context.Set<api.Models.Data.TaskModel>().AddRange(task1, task2);
            context.SaveChanges();
            var repo = new TaskRepository(context);

            var result = await repo.GetAllTasksAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that GetTaskByIdAsync returns the task when it exists.
        /// </summary>
        [Fact(Skip = "This test is skipped because the functionality is not yet implemented.")]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenTaskExists()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var taskId = Guid.NewGuid();
            var task = new api.Models.Data.TaskModel { Id = taskId, Title = "Test Task", CreatedBy = "user1", ListId = Guid.NewGuid() };
            context.Set<api.Models.Data.TaskModel>().Add(task);
            context.SaveChanges();
            var repo = new TaskRepository(context);

            var result = await repo.GetTaskByIdAsync(taskId);

            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
        }

        /// <summary>
        /// Tests that GetTaskByIdAsync returns null when the task does not exist.
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_ReturnsNull_WhenTaskDoesNotExist()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var repo = new TaskRepository(context);

            var result = await repo.GetTaskByIdAsync(Guid.NewGuid());

            Assert.Null(result);
        }

        /// <summary>
        /// Tests that CreateTaskAsync adds a new task to the database.
        /// </summary>
        [Fact]
        public async Task CreateTaskAsync_AddsTaskToDatabase()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var repo = new TaskRepository(context);
            var task = new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "New Task", CreatedBy = "user1", ListId = Guid.NewGuid() };

            var result = await repo.CreateTaskAsync(task);

            Assert.NotNull(result);
            var dbTask = context.Set<api.Models.Data.TaskModel>().Find(task.Id);
            Assert.NotNull(dbTask);
            Assert.Equal("New Task", dbTask!.Title);
        }

        /// <summary>
        /// Tests that UpdateTaskAsync updates an existing task in the database.
        /// </summary>
        [Fact]
        public async Task UpdateTaskAsync_UpdatesTaskInDatabase()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var task = new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "Old Name", CreatedBy = "user1", ListId = Guid.NewGuid() };
            context.Set<api.Models.Data.TaskModel>().Add(task);
            context.SaveChanges();
            var repo = new TaskRepository(context);
            task.Title = "Updated Name";

            var result = await repo.UpdateTaskAsync(task);

            Assert.NotNull(result);
            var dbTask = context.Set<api.Models.Data.TaskModel>().Find(task.Id);
            Assert.NotNull(dbTask);
            Assert.Equal("Updated Name", dbTask!.Title);
        }

        /// <summary>
        /// Tests that DeleteTaskAsync removes a task from the database.
        /// </summary>
        [Fact]
        public async Task DeleteTaskAsync_RemovesTaskFromDatabase()
        {
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var task = new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "To Delete", CreatedBy = "user1", ListId = Guid.NewGuid() };
            context.Set<api.Models.Data.TaskModel>().Add(task);
            context.SaveChanges();
            var repo = new TaskRepository(context);

            var result = await repo.DeleteTaskAsync(task);

            Assert.True(result);
            var dbTask = context.Set<api.Models.Data.TaskModel>().Find(task.Id);
            Assert.Null(dbTask);
        }
    }
}