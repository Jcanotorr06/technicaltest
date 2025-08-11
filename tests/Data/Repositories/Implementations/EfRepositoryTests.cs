using Microsoft.EntityFrameworkCore;
using api.Data.Context;
using api.Models.Data;
using Xunit;
using Assert = Xunit.Assert;

namespace api.Data.Repositories.Implementations.Tests
{
    /// <summary>
    /// Unit tests for the EfRepository class using an in-memory database.
    /// </summary>
    public class EfRepositoryTests
    {
        /// <summary>
        /// The in-memory TaskContext used for testing.
        /// </summary>
        private readonly TaskContext _context;

        /// <summary>
        /// The EfRepository instance under test.
        /// </summary>
        private readonly EfRepository<TaskModel> _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="EfRepositoryTests"/> class and sets up the in-memory database.
        /// </summary>
        public EfRepositoryTests()
        {
            // Configure the in-memory database for isolation between tests
            var options = new DbContextOptionsBuilder<TaskContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new TaskContext(options);
            _repository = new EfRepository<TaskModel>(_context);
        }

        /// <summary>
        /// Tests that GetByIdAsync returns the entity when it exists in the database.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsEntity_WhenEntityExists()
        {
            // Arrange: Add a new TaskModel to the in-memory database
            var id = Guid.NewGuid();
            var task = new TaskModel { Id = id, Title = "Test Task", CreatedBy = "user", AssignedTo = "user", ListId = Guid.NewGuid() };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act: Retrieve the entity by its ID
            var result = await _repository.GetByIdAsync(id);

            // Assert: The entity should be found and IDs should match
            Assert.NotNull(result);
            Assert.Equal(id, result.Id);
        }

        /// <summary>
        /// Tests that GetByIdAsync returns null when the entity does not exist.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ReturnsNull_WhenEntityDoesNotExist()
        {
            // Arrange: Use a random ID that does not exist
            var id = Guid.NewGuid();

            // Act: Try to retrieve the entity
            var result = await _repository.GetByIdAsync(id);

            // Assert: The result should be null
            Assert.Null(result);
        }

        /// <summary>
        /// Tests that AddAsync adds a new entity to the database.
        /// </summary>
        [Fact]
        public async Task AddAsync_AddsEntityToDatabase()
        {
            // Arrange: Create a new TaskModel
            var task = new TaskModel { Id = Guid.NewGuid(), Title = "Add Test", CreatedBy = "user", AssignedTo = "user", ListId = Guid.NewGuid() };

            // Act: Add the entity using the repository
            var result = await _repository.AddAsync(task);

            // Assert: The entity should be present in the database
            var dbTask = await _context.Tasks.FindAsync(task.Id);
            Assert.NotNull(dbTask);
            Assert.Equal(task.Title, dbTask.Title);
        }

        /// <summary>
        /// Tests that UpdateAsync updates an existing entity in the database.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_UpdatesEntityInDatabase()
        {
            // Arrange: Add a TaskModel and then modify it
            var task = new TaskModel { Id = Guid.NewGuid(), Title = "Original", CreatedBy = "user", AssignedTo = "user", ListId = Guid.NewGuid() };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Change the title
            task.Title = "Updated";

            // Act: Update the entity using the repository
            var result = await _repository.UpdateAsync(task);

            // Assert: The entity in the database should reflect the update
            var dbTask = await _context.Tasks.FindAsync(task.Id);
            Assert.Equal("Updated", dbTask.Title);
        }

        /// <summary>
        /// Tests that DeleteAsync removes an entity from the database.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_RemovesEntityFromDatabase()
        {
            // Arrange: Add a TaskModel to the database
            var task = new TaskModel { Id = Guid.NewGuid(), Title = "Delete Test", CreatedBy = "user", AssignedTo = "user", ListId = Guid.NewGuid() };
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            // Act: Delete the entity using the repository
            var result = await _repository.DeleteAsync(task);

            // Assert: The entity should be removed from the database
            Assert.True(result);
            var dbTask = await _context.Tasks.FindAsync(task.Id);
            Assert.Null(dbTask);
        }

        /// <summary>
        /// Tests that DeleteAsync returns false when the entity is null.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ReturnsFalse_WhenEntityIsNull()
        {
            // Act: Attempt to delete a null entity
            var result = await _repository.DeleteAsync(null);

            // Assert: The result should be false
            Assert.False(result);
        }
    }
}