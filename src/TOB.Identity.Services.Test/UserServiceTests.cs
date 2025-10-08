using Moq;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Repositories;
using Xunit;

namespace TOB.Identity.Services.Test;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IUserRoleRepository> _mockUserRoleRepository;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockUserRoleRepository = new Mock<IUserRoleRepository>();
    }

    [Fact]
    public async Task DoesUsernameExistsAsync_WhenUsernameExists_ReturnsTrue()
    {
        // Arrange
        var username = "testuser@test.com";
        _mockUserRepository.Setup(x => x.DoesUsernameExistsAsync(username))
            .ReturnsAsync(true);

        // Act
        var result = await _mockUserRepository.Object.DoesUsernameExistsAsync(username);

        // Assert
        Assert.True(result);
        _mockUserRepository.Verify(x => x.DoesUsernameExistsAsync(username), Times.Once);
    }

    [Fact]
    public async Task DoesUsernameExistsAsync_WhenUsernameDoesNotExist_ReturnsFalse()
    {
        // Arrange
        var username = "nonexistent@test.com";
        _mockUserRepository.Setup(x => x.DoesUsernameExistsAsync(username))
            .ReturnsAsync(false);

        // Act
        var result = await _mockUserRepository.Object.DoesUsernameExistsAsync(username);

        // Assert
        Assert.False(result);
        _mockUserRepository.Verify(x => x.DoesUsernameExistsAsync(username), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WhenCalled_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var userDto = new UserDto { UserId = userId, Email = "test@test.com", FirstName = "John", LastName = "Doe" };

        _mockUserRepository.Setup(x => x.GetUserByIdAsync(userId))
            .ReturnsAsync(userDto);

        // Act
        var result = await _mockUserRepository.Object.GetUserByIdAsync(userId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal("test@test.com", result.Email);
        _mockUserRepository.Verify(x => x.GetUserByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetTenantUsersAsync_WhenCalled_ReturnsAllUsers()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var users = new List<UserDto>
        {
            new UserDto { UserId = Guid.NewGuid(), Email = "user1@test.com", TenantId = tenantId },
            new UserDto { UserId = Guid.NewGuid(), Email = "user2@test.com", TenantId = tenantId }
        };

        _mockUserRepository.Setup(x => x.GetTenantUsersAsync(tenantId))
            .ReturnsAsync(users);

        // Act
        var result = await _mockUserRepository.Object.GetTenantUsersAsync(tenantId);

        // Assert
        Assert.Equal(2, result.Count());
        _mockUserRepository.Verify(x => x.GetTenantUsersAsync(tenantId), Times.Once);
    }
}
