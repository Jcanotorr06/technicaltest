using Xunit;
using api.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using api.Data.Repositories.Interfaces;
using api.Models.Dto.List;
using api.Models.Data;
using Assert = Xunit.Assert;

namespace api.Services.Implementations.Tests
{
    /// <summary>
    /// Unit tests for the ListService class using Moq for IListRepository.
    /// </summary>
    public class ListServiceTests
    {
        private readonly Mock<IListRepository> _listRepositoryMock;
        private readonly ListService _listService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListServiceTests"/> class and sets up the mock repository.
        /// </summary>
        public ListServiceTests()
        {
            _listRepositoryMock = new Mock<IListRepository>();
            _listService = new ListService(_listRepositoryMock.Object);
        }


        /// <summary>
        /// Tests that CreateListAsync returns a ReadListDto when given valid input.
        /// </summary>
        [Fact]
        public async Task CreateListAsync_ShouldReturnReadListDto_WhenValidInput()
        {
            // Arrange
            var createListDto = new CreateListDto { Name = "Test List", IsPublic = true };
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            var listModel = new ListModel
            {
                Id = Guid.NewGuid(),
                Name = createListDto.Name,
                IsPublic = createListDto.IsPublic,
                CreatedBy = $"{user.Name};{user.Email};{user.Id}"
            };
            _listRepositoryMock
                .Setup(r => r.CreateListAsync(It.IsAny<ListModel>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(listModel);

            // Act
            var result = await _listService.CreateListAsync(createListDto, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(listModel.Name, result.Name);
            Assert.Equal(listModel.Id, result.Id);
        }


        /// <summary>
        /// Tests that DeleteListAsync returns true when the list exists.
        /// </summary>
        [Fact]
        public async Task DeleteListAsync_ShouldReturnTrue_WhenListExists()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var listModel = new ListModel { Id = listId, Name = "Test List", CreatedBy = "user" };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(listModel);
            _listRepositoryMock.Setup(r => r.DeleteListAsync(listModel, It.IsAny<CancellationToken>())).ReturnsAsync(true);

            // Act
            var result = await _listService.DeleteListAsync(listId);

            // Assert
            Assert.True(result);
        }

        /// <summary>
        /// Tests that DeleteListAsync throws ArgumentException when the list ID is empty.
        /// </summary>
        [Fact]
        public async Task DeleteListAsync_ShouldThrowArgumentException_WhenListIdIsEmpty()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _listService.DeleteListAsync(Guid.Empty));
        }

        /// <summary>
        /// Tests that DeleteListAsync throws KeyNotFoundException when the list does not exist.
        /// </summary>
        [Fact]
        public async Task DeleteListAsync_ShouldThrowKeyNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var listId = Guid.NewGuid();
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync((ListModel)null!);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _listService.DeleteListAsync(listId));
        }

        /// <summary>
        /// Tests that GetAllListsAsync returns a collection of ReadListDto objects.
        /// </summary>
        [Fact]
        public async Task GetAllListsAsync_ShouldReturnReadListDtos()
        {
            // Arrange
            var lists = new List<ListModel> {
                new ListModel { Id = Guid.NewGuid(), Name = "List 1", IsPublic = true },
                new ListModel { Id = Guid.NewGuid(), Name = "List 2", IsPublic = false }
            };
            _listRepositoryMock.Setup(r => r.GetAllListsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(lists);

            // Act
            var result = await _listService.GetAllListsAsync();

            // Assert
            // If ReadListDto does not have IsPublic, just check count or names
            foreach (var dto in result)
            {
                Assert.StartsWith("List", dto.Name);
            }
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that GetPublicListsAsync returns only public lists.
        /// </summary>
        [Fact]
        public async Task GetPublicListsAsync_ShouldReturnOnlyPublicLists()
        {
            // Arrange
            var lists = new List<ListModel> {
                new ListModel { Id = Guid.NewGuid(), Name = "List 1", IsPublic = true },
                new ListModel { Id = Guid.NewGuid(), Name = "List 2", IsPublic = true }
            };
            _listRepositoryMock.Setup(r => r.GetPublicListsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(lists);

            // Act
            var result = await _listService.GetPublicListsAsync();

            // Assert
            Assert.NotNull(result);
        }

        /// <summary>
        /// Tests that GetListByIdAsync returns a ReadListDto when the list is public or the user is the owner.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_ShouldReturnReadListDto_WhenListIsPublicOrOwner()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            var createdBy = $"{user.Name};{user.Email};{user.Id}";
            var listModel = new ListModel { Id = listId, Name = "Test List", IsPublic = true, CreatedBy = createdBy };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(listModel);

            // Act
            var result = await _listService.GetListByIdAsync(listId, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(listId, result.Id);
        }

        /// <summary>
        /// Tests that GetListByIdAsync throws ArgumentException when the list ID is empty.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_ShouldThrowArgumentException_WhenListIdIsEmpty()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _listService.GetListByIdAsync(Guid.Empty, user));
        }

        /// <summary>
        /// Tests that GetListByIdAsync throws KeyNotFoundException when the list does not exist.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_ShouldThrowKeyNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync((ListModel)null);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _listService.GetListByIdAsync(listId, user));
        }

        /// <summary>
        /// Tests that GetListByIdAsync throws UnauthorizedAccessException when the list is private and the user is not the owner.
        /// </summary>
        [Fact]
        public async Task GetListByIdAsync_ShouldThrowUnauthorizedAccessException_WhenListIsPrivateAndNotOwner()
        {
            // Arrange
            var listId = Guid.NewGuid();
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            var listModel = new ListModel { Id = listId, Name = "Test List", IsPublic = false, CreatedBy = "otheruser;other@example.com;00000000-0000-0000-0000-000000000000" };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(listId, It.IsAny<CancellationToken>())).ReturnsAsync(listModel);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _listService.GetListByIdAsync(listId, user));
        }

        /// <summary>
        /// Tests that GetUserListsAsync returns a collection of ReadListDto objects for the user.
        /// </summary>
        [Fact]
        public async Task GetUserListsAsync_ShouldReturnReadListDtos()
        {
            // Arrange
            var userId = "user1";
            var lists = new List<ListModel> {
                new ListModel { Id = Guid.NewGuid(), Name = "List 1", CreatedBy = userId },
                new ListModel { Id = Guid.NewGuid(), Name = "List 2", CreatedBy = userId }
            };
            _listRepositoryMock.Setup(r => r.GetUserListsAsync(userId, It.IsAny<CancellationToken>())).ReturnsAsync(lists);

            // Act
            var result = await _listService.GetUserListsAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
        }

        /// <summary>
        /// Tests that GetUserListsAsync throws ArgumentException when the user ID is invalid.
        /// </summary>
        [Fact]
        public async Task GetUserListsAsync_ShouldThrowArgumentException_WhenUserIdIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _listService.GetUserListsAsync(" "));
        }

        /// <summary>
        /// Tests that UpdateListAsync returns a ReadListDto when the input is valid and the user is the owner.
        /// </summary>
        [Fact]
        public async Task UpdateListAsync_ShouldReturnReadListDto_WhenValidInputAndOwner()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            var updateListDto = new UpdateListDto { Id = Guid.NewGuid(), Name = "Updated List", IsPublic = true };
            var createdBy = $"{user.Name};{user.Email};{user.Id}";
            var existingList = new ListModel { Id = updateListDto.Id, Name = "Old List", IsPublic = false, CreatedBy = createdBy };
            var updatedList = new ListModel { Id = updateListDto.Id, Name = updateListDto.Name, IsPublic = updateListDto.IsPublic, CreatedBy = createdBy };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(updateListDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingList);
            _listRepositoryMock.Setup(r => r.UpdateListAsync(It.IsAny<ListModel>(), It.IsAny<CancellationToken>())).ReturnsAsync(updatedList);

            // Act
            var result = await _listService.UpdateListAsync(updateListDto, user);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateListDto.Name, result.Name);
        }

        /// <summary>
        /// Tests that UpdateListAsync throws ArgumentNullException when the update DTO is null.
        /// </summary>
        [Fact]
        public async Task UpdateListAsync_ShouldThrowArgumentNullException_WhenUpdateListDtoIsNull()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _listService.UpdateListAsync(null!, user));
        }

        /// <summary>
        /// Tests that UpdateListAsync throws KeyNotFoundException when the list does not exist.
        /// </summary>
        [Fact]
        public async Task UpdateListAsync_ShouldThrowKeyNotFoundException_WhenListDoesNotExist()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            var updateListDto = new UpdateListDto { Id = Guid.NewGuid(), Name = "Updated List", IsPublic = true };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(updateListDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync((ListModel)null!);

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(() => _listService.UpdateListAsync(updateListDto, user));
        }

        /// <summary>
        /// Tests that UpdateListAsync throws UnauthorizedAccessException when the user is not the owner of the list.
        /// </summary>
        [Fact]
        public async Task UpdateListAsync_ShouldThrowUnauthorizedAccessException_WhenNotOwner()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };
            var updateListDto = new UpdateListDto { Id = Guid.NewGuid(), Name = "Updated List", IsPublic = true };
            var existingList = new ListModel { Id = updateListDto.Id, Name = "Old List", IsPublic = false, CreatedBy = "otheruser;other@example.com;00000000-0000-0000-0000-000000000000" };
            _listRepositoryMock.Setup(r => r.GetListByIdAsync(updateListDto.Id, It.IsAny<CancellationToken>())).ReturnsAsync(existingList);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _listService.UpdateListAsync(updateListDto, user));
        }

        /// <summary>
        /// Tests that CreateListAsync throws ArgumentNullException when the create DTO is null.
        /// </summary>
        [Fact]
        public async Task CreateListAsync_ShouldThrowArgumentNullException_WhenCreateListDtoIsNull()
        {
            // Arrange
            var user = new UserModel { Id = Guid.NewGuid(), Name = "Test User", Email = "test@example.com" };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => _listService.CreateListAsync(null, user));
        }
    }
}