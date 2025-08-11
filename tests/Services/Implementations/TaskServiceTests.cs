using Xunit;
using Moq;
using Assert = Xunit.Assert;
using api.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace api.Services.Implementations.Tests
{
    /// <summary>
    /// Unit tests for the TaskService class using Moq for repository dependencies.
    /// </summary>
    public class TaskServiceTests
    {
        private readonly Moq.Mock<api.Data.Repositories.Interfaces.ITaskRepository> _taskRepositoryMock = new();
        private readonly Moq.Mock<api.Data.Repositories.Interfaces.IListRepository> _listRepositoryMock = new();
        private readonly TaskService _taskService;

        /// <summary>
        /// Initializes a new instance of the <see cref="TaskServiceTests"/> class and sets up the mock repositories.
        /// </summary>
        public TaskServiceTests()
        {
            _taskService = new TaskService(_taskRepositoryMock.Object, _listRepositoryMock.Object);
        }

        /// <summary>
        /// Tests that GetAllTasksAsync returns all tasks in the database.
        /// </summary>
        [Fact]
        public async Task GetAllTasksAsync_ReturnsAllTasks()
        {
            // Arrange
            var tasks = new List<api.Models.Data.TaskModel> {
                new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "Task 1" },
                new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = "Task 2" }
            };
            _taskRepositoryMock.Setup(r => r.GetAllTasksAsync(It.IsAny<CancellationToken>())).ReturnsAsync(tasks);

            // Act
            var result = await _taskService.GetAllTasksAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that GetTaskByIdAsync returns the task when it exists and the user is the owner or the list is public.
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_ReturnsTask_WhenTaskExistsAndUserIsOwnerOrPublic()
        {
            // Arrange
            var taskId = Guid.NewGuid();
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var list = new api.Models.Data.ListModel { Id = Guid.NewGuid(), IsPublic = true, CreatedBy = $"{user.Name};{user.Email};{user.Id}" };
            var task = new api.Models.Data.TaskModel { Id = taskId, Title = "Test Task", _List = list };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);

            // Act
            var result = await _taskService.GetTaskByIdAsync(taskId, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(taskId, result.Id);
        }

        /// <summary>
        /// Tests that GetTaskByIdAsync throws ArgumentException when the task ID is empty.
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_ThrowsArgumentException_WhenTaskIdIsEmpty()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            await Assert.ThrowsAsync<ArgumentException>(() => _taskService.GetTaskByIdAsync(Guid.Empty, user));
        }

        /// <summary>
        /// Tests that GetTaskByIdAsync throws KeyNotFoundException when the task does not exist.
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_ThrowsKeyNotFoundException_WhenTaskDoesNotExist()
        {
            var taskId = Guid.NewGuid();
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync((api.Models.Data.TaskModel)null!);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.GetTaskByIdAsync(taskId, user));
        }

        /// <summary>
        /// Tests that GetTaskByIdAsync throws UnauthorizedAccessException when the user is not the owner and the list is not public.
        /// </summary>
        [Fact]
        public async Task GetTaskByIdAsync_ThrowsUnauthorizedAccessException_WhenNotOwnerAndNotPublic()
        {
            var taskId = Guid.NewGuid();
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var list = new api.Models.Data.ListModel { Id = Guid.NewGuid(), IsPublic = false, CreatedBy = "other;other@example.com;00000000-0000-0000-0000-000000000000" };
            var task = new api.Models.Data.TaskModel { Id = taskId, Title = "Test Task", _List = list };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(task);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _taskService.GetTaskByIdAsync(taskId, user));
        }
        /// <summary>
        /// Tests that CreateTaskAsync returns a ReadTaskDto when given valid input.
        /// </summary>
        [Fact]
        public async Task CreateTaskAsync_ReturnsReadTaskDto_WhenValidInput()
        {
            // Arrange
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var createTaskDto = new api.Models.Dto.Task.CreateTaskDto { Title = "New Task", ListId = Guid.NewGuid(), Status = 1 };
            var taskModel = new api.Models.Data.TaskModel { Id = Guid.NewGuid(), Title = createTaskDto.Title, ListId = createTaskDto.ListId, Status = createTaskDto.Status, CreatedBy = $"{user.Name};{user.Email};{user.Id}", AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            _taskRepositoryMock.Setup(r => r.CreateTaskAsync(It.IsAny<api.Models.Data.TaskModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(taskModel);

            // Act
            var result = await _taskService.CreateTaskAsync(createTaskDto, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createTaskDto.Title, result.Title);
        }

        /// <summary>
        /// Tests that CreateTaskAsync throws ArgumentNullException when the input is null.
        /// </summary>
        [Fact]
        public async Task CreateTaskAsync_ThrowsArgumentNullException_WhenInputIsNull()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            await Assert.ThrowsAsync<ArgumentNullException>(() => _taskService.CreateTaskAsync(null!, user));
        }

        /// <summary>
        /// Tests that UpdateTaskAsync returns a ReadTaskDto when the input is valid and the user is the owner.
        /// </summary>
        [Fact]
        public async Task UpdateTaskAsync_ReturnsReadTaskDto_WhenValidInputAndOwner()
        {
            // Arrange
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var updateTaskDto = new api.Models.Dto.Task.UpdateTaskDto { Id = Guid.NewGuid(), Title = "Updated Task", Status = 1, AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            var existingTask = new api.Models.Data.TaskModel { Id = updateTaskDto.Id, Title = "Old Task", Status = 0, CreatedBy = $"{user.Name};{user.Email};{user.Id}", AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            var updatedTask = new api.Models.Data.TaskModel { Id = updateTaskDto.Id, Title = updateTaskDto.Title, Status = updateTaskDto.Status, CreatedBy = existingTask.CreatedBy, AssignedTo = updateTaskDto.AssignedTo };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(updateTaskDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingTask);
            _taskRepositoryMock.Setup(r => r.UpdateTaskAsync(It.IsAny<api.Models.Data.TaskModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedTask);

            // Act
            var result = await _taskService.UpdateTaskAsync(updateTaskDto, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateTaskDto.Title, result.Title);
        }

        /// <summary>
        /// Tests that UpdateTaskAsync throws ArgumentNullException when the input is null.
        /// </summary>
        [Fact]
        public async Task UpdateTaskAsync_ThrowsArgumentNullException_WhenInputIsNull()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            await Assert.ThrowsAsync<ArgumentNullException>(() => _taskService.UpdateTaskAsync(null!, user));
        }

        /// <summary>
        /// Tests that UpdateTaskAsync throws KeyNotFoundException when the task does not exist.
        /// </summary>
        [Fact]
        public async Task UpdateTaskAsync_ThrowsKeyNotFoundException_WhenTaskDoesNotExist()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var updateTaskDto = new api.Models.Dto.Task.UpdateTaskDto { Id = Guid.NewGuid(), Title = "Updated Task", Status = 1, AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(updateTaskDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync((api.Models.Data.TaskModel)null!);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.UpdateTaskAsync(updateTaskDto, user));
        }

        /// <summary>
        /// Tests that UpdateTaskAsync throws UnauthorizedAccessException when the user is not the owner of the task.
        /// </summary>
        [Fact]
        public async Task UpdateTaskAsync_ThrowsUnauthorizedAccessException_WhenNotOwner()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var updateTaskDto = new api.Models.Dto.Task.UpdateTaskDto { Id = Guid.NewGuid(), Title = "Updated Task", Status = 1, AssignedTo = "other;other@example.com;00000000-0000-0000-0000-000000000000" };
            var existingTask = new api.Models.Data.TaskModel { Id = updateTaskDto.Id, Title = "Old Task", Status = 0, CreatedBy = "other;other@example.com;00000000-0000-0000-0000-000000000000", AssignedTo = "other;other@example.com;00000000-0000-0000-0000-000000000000" };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(updateTaskDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingTask);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _taskService.UpdateTaskAsync(updateTaskDto, user));
        }

        /// <summary>
        /// Tests that UpdateTaskAsync throws InvalidOperationException when the task is already completed.
        /// </summary>
        [Fact]
        public async Task UpdateTaskAsync_ThrowsInvalidOperationException_WhenTaskIsCompleted()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var updateTaskDto = new api.Models.Dto.Task.UpdateTaskDto { Id = Guid.NewGuid(), Title = "Updated Task", Status = 1, AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            var existingTask = new api.Models.Data.TaskModel { Id = updateTaskDto.Id, Title = "Old Task", Status = (int)1, CreatedBy = $"{user.Name};{user.Email};{user.Id}", AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            // Simulate completed status
            existingTask.Status = (int)Models.Static.TaskStatusEnum.Completed;
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(updateTaskDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingTask);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _taskService.UpdateTaskAsync(updateTaskDto, user));
        }

        /// <summary>
        /// Tests that DeleteTaskAsync returns true when the input is valid and the user is the owner.
        /// </summary>
        [Fact]
        public async Task DeleteTaskAsync_ReturnsTrue_WhenValidInputAndOwner()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var taskId = Guid.NewGuid();
            var existingTask = new api.Models.Data.TaskModel { Id = taskId, Status = 1, CreatedBy = $"{user.Name};{user.Email};{user.Id}", AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTask);
            _taskRepositoryMock.Setup(r => r.DeleteTaskAsync(existingTask, It.IsAny<CancellationToken>())).ReturnsAsync(true);
            var result = await _taskService.DeleteTaskAsync(taskId, user);
            Assert.True(result);
        }

        /// <summary>
        /// Tests that DeleteTaskAsync throws ArgumentException when the task ID is empty.
        /// </summary>
        [Fact]
        public async Task DeleteTaskAsync_ThrowsArgumentException_WhenTaskIdIsEmpty()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            await Assert.ThrowsAsync<ArgumentException>(() => _taskService.DeleteTaskAsync(Guid.Empty, user));
        }

        /// <summary>
        /// Tests that DeleteTaskAsync throws KeyNotFoundException when the task does not exist.
        /// </summary>
        [Fact]
        public async Task DeleteTaskAsync_ThrowsKeyNotFoundException_WhenTaskDoesNotExist()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var taskId = Guid.NewGuid();
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync((api.Models.Data.TaskModel)null!);
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _taskService.DeleteTaskAsync(taskId, user));
        }

        /// <summary>
        /// Tests that DeleteTaskAsync throws UnauthorizedAccessException when the user is not the owner of the task.
        /// </summary>
        [Fact]
        public async Task DeleteTaskAsync_ThrowsUnauthorizedAccessException_WhenNotOwner()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var taskId = Guid.NewGuid();
            var existingTask = new api.Models.Data.TaskModel { Id = taskId, Status = 1, CreatedBy = "other;other@example.com;00000000-0000-0000-0000-000000000000", AssignedTo = "other;other@example.com;00000000-0000-0000-0000-000000000000" };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTask);
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _taskService.DeleteTaskAsync(taskId, user));
        }

        /// <summary>
        /// Tests that DeleteTaskAsync throws InvalidOperationException when the task is already completed.
        /// </summary>
        [Fact]
        public async Task DeleteTaskAsync_ThrowsInvalidOperationException_WhenTaskIsCompleted()
        {
            var user = new api.Models.Data.UserModel { Id = Guid.NewGuid(), Name = "User", Email = "user@example.com" };
            var taskId = Guid.NewGuid();
            var existingTask = new api.Models.Data.TaskModel { Id = taskId, Status = (int)Models.Static.TaskStatusEnum.Completed, CreatedBy = $"{user.Name};{user.Email};{user.Id}", AssignedTo = $"{user.Name};{user.Email};{user.Id}" };
            _taskRepositoryMock.Setup(r => r.GetTaskByIdAsync(taskId, It.IsAny<CancellationToken>())).ReturnsAsync(existingTask);
            await Assert.ThrowsAsync<InvalidOperationException>(() => _taskService.DeleteTaskAsync(taskId, user));
        }
    }
}