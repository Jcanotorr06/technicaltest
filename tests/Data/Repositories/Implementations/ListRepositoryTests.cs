using Xunit;
using api.Data.Context;
using Microsoft.EntityFrameworkCore;
using api.Models.Data;
using Assert = Xunit.Assert;

namespace api.Data.Repositories.Implementations.Tests
{
    /// <summary>
    /// Unit tests for the ListRepository class using an in-memory database.
    /// </summary>
    public class ListRepositoryTests
    {
        /// <summary>
        /// Creates a new in-memory TaskContext for test isolation.
        /// </summary>
        /// <param name="dbName">The unique database name for the in-memory context.</param>
        /// <returns>A new TaskContext instance.</returns>
        private static TaskContext CreateInMemoryContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;
            return new TaskContext(options);
        }

        /// <summary>
        /// Tests that GetAllListsAsync returns all lists in the database.
        /// </summary>
        [Fact]
        public async Task GetAllListsAsync_ReturnsAllLists()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var list1 = new ListModel { Id = Guid.NewGuid(), Name = "List 1", CreatedBy = "user1", IsPublic = true, Tasks = new List<TaskModel>() };
            var list2 = new ListModel { Id = Guid.NewGuid(), Name = "List 2", CreatedBy = "user2", IsPublic = false, Tasks = new List<TaskModel>() };
            context.Set<ListModel>().AddRange(list1, list2);
            context.SaveChanges();
            var repo = new ListRepository(context);

            // Act
            var result = await repo.GetAllListsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that GetUserListsAsync returns only the lists created by the specified user.
        /// </summary>
        [Fact]
        public async Task GetUserListsAsync_ReturnsListsForUser()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var userId = "user1";
            var list1 = new ListModel { Id = Guid.NewGuid(), Name = "List 1", CreatedBy = userId, IsPublic = true, Tasks = new List<TaskModel>() };
            var list2 = new ListModel { Id = Guid.NewGuid(), Name = "List 2", CreatedBy = "user2", IsPublic = false, Tasks = new List<TaskModel>() };
            context.Set<ListModel>().AddRange(list1, list2);
            context.SaveChanges();
            var repo = new ListRepository(context);

            // Act
            var result = await repo.GetUserListsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(userId, result.First().CreatedBy);
        }

        /// <summary>
        /// Tests that GetPublicListsAsync returns only public lists.
        /// </summary>
        [Fact]
        public async Task GetPublicListsAsync_ReturnsOnlyPublicLists()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var list1 = new ListModel { Id = Guid.NewGuid(), Name = "List 1", CreatedBy = "user1", IsPublic = true, Tasks = new List<TaskModel>() };
            var list2 = new ListModel { Id = Guid.NewGuid(), Name = "List 2", CreatedBy = "user2", IsPublic = false, Tasks = new List<TaskModel>() };
            context.Set<ListModel>().AddRange(list1, list2);
            context.SaveChanges();
            var repo = new ListRepository(context);

            // Act
            var result = await repo.GetPublicListsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.True(result.First().IsPublic);
        }

        /// <summary>
        /// Tests that CreateListAsync adds a new list to the database.
        /// </summary>
        [Fact]
        public async Task CreateListAsync_AddsListToDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var repo = new ListRepository(context);
            var list = new ListModel { Id = Guid.NewGuid(), Name = "New List", CreatedBy = "user1", IsPublic = false, Tasks = new List<TaskModel>() };

            // Act
            var result = await repo.CreateListAsync(list);

            // Assert
            Assert.NotNull(result);
            var dbList = context.Set<ListModel>().Find(list.Id);
            Assert.NotNull(dbList);
            Assert.Equal("New List", dbList.Name);
        }

        /// <summary>
        /// Tests that UpdateListAsync updates an existing list in the database.
        /// </summary>
        [Fact]
        public async Task UpdateListAsync_UpdatesListInDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var list = new ListModel { Id = Guid.NewGuid(), Name = "Old Name", CreatedBy = "user1", IsPublic = false, Tasks = new List<TaskModel>() };
            context.Set<ListModel>().Add(list);
            context.SaveChanges();
            var repo = new ListRepository(context);
            list.Name = "Updated Name";

            // Act
            var result = await repo.UpdateListAsync(list);

            // Assert
            Assert.NotNull(result);
            var dbList = context.Set<ListModel>().Find(list.Id);
            Assert.NotNull(dbList);
            Assert.Equal("Updated Name", dbList.Name);
        }

        /// <summary>
        /// Tests that DeleteListAsync removes a list from the database.
        /// </summary>
        [Fact]
        public async Task DeleteListAsync_RemovesListFromDatabase()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var list = new ListModel { Id = Guid.NewGuid(), Name = "To Delete", CreatedBy = "user1", IsPublic = false, Tasks = new List<TaskModel>() };
            context.Set<ListModel>().Add(list);
            context.SaveChanges();
            var repo = new ListRepository(context);

            // Act
            var result = await repo.DeleteListAsync(list);

            // Assert
            Assert.True(result);
            var dbList = context.Set<ListModel>().Find(list.Id);
            Assert.Null(dbList);
        }

        /// <summary>
        /// Tests that GetListByIdAsync returns the list when it exists.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_ReturnsList_WhenListExists()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var listId = Guid.NewGuid();
            var list = new ListModel
            {
                Id = listId,
                Name = "Test List",
                CreatedBy = "user1",
                IsPublic = true,
                Tasks = new List<TaskModel>()
            };
            context.Set<ListModel>().Add(list);
            context.SaveChanges();

            var repo = new ListRepository(context);

            // Act
            var result = await repo.GetListByIdAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(listId, result.Id);
            Assert.Equal("Test List", result.Name);
        }

        /// <summary>
        /// Tests that GetListByIdAsync returns null when the list does not exist.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_ReturnsNull_WhenListDoesNotExist()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var repo = new ListRepository(context);

            // Act
            var result = await repo.GetListByIdAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that GetListByIdAsync includes related tasks in the returned list.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_IncludesTasks()
        {
            // Arrange
            var dbName = Guid.NewGuid().ToString();
            var context = CreateInMemoryContext(dbName);
            var listId = Guid.NewGuid();
            var task = new TaskModel
            {
                Id = Guid.NewGuid(),
                Title = "Task 1",
                CreatedBy = "user1",
                ListId = listId
            };
            var list = new ListModel
            {
                Id = listId,
                Name = "List with Tasks",
                CreatedBy = "user1",
                IsPublic = false,
                Tasks = new List<TaskModel> { task }
            };
            context.Set<ListModel>().Add(list);
            context.Set<TaskModel>().Add(task);
            context.SaveChanges();

            var repo = new ListRepository(context);

            // Act
            var result = await repo.GetListByIdAsync(listId);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Tasks);
            Assert.Single(result.Tasks);
            Assert.Equal("Task 1", result.Tasks.First().Title);
        }

    }
}