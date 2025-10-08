using AutoMapper;
using Moq;
using TOB.Identity.Domain;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Infrastructure.Repositories;
using TOB.Identity.Services.Implementations;
using Xunit;

namespace TOB.Identity.Services.Test;

public class TenantServiceTests
{
    private readonly Mock<ITenantRepository> _mockTenantRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly TenantService _tenantService;

    public TenantServiceTests()
    {
        _mockTenantRepository = new Mock<ITenantRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockUserService = new Mock<IUserService>();
        _mockMapper = new Mock<IMapper>();

        _tenantService = new TenantService(
            _mockTenantRepository.Object,
            _mockRoleRepository.Object,
            _mockUserService.Object,
            _mockMapper.Object
        );
    }

    [Fact]
    public async Task GetAllTenantsAsync_WhenCalled_ReturnsAllTenants()
    {
        // Arrange
        var tenants = new List<TenantDto>
        {
            new TenantDto { TenantId = Guid.NewGuid(), TenantName = "Tenant 1" },
            new TenantDto { TenantId = Guid.NewGuid(), TenantName = "Tenant 2" }
        };

        _mockTenantRepository.Setup(x => x.GetAllTenantsAsync())
            .ReturnsAsync(tenants);

        // Act
        var result = await _tenantService.GetAllTenantsAsync();

        // Assert
        Assert.Equal(2, result.Count());
        _mockTenantRepository.Verify(x => x.GetAllTenantsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTenantByIdAsync_WhenTenantExists_ReturnsTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new TenantDto { TenantId = tenantId, TenantName = "Test Tenant" };

        _mockTenantRepository.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(tenant);

        // Act
        var result = await _tenantService.GetTenantByIdAsync(tenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(tenantId, result.TenantId);
        Assert.Equal("Test Tenant", result.TenantName);
        _mockTenantRepository.Verify(x => x.GetTenantByIdAsync(tenantId), Times.Once);
    }

    [Fact]
    public async Task DeleteTenantAsync_WhenCalled_DeletesTenantAndUsers()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var deletedBy = Guid.NewGuid();

        _mockTenantRepository.Setup(x => x.DeleteTenantAsync(tenantId, deletedBy))
            .ReturnsAsync(true);
        _mockUserService.Setup(x => x.DeleteTenantUsersAsync(tenantId, deletedBy))
            .ReturnsAsync(true);

        // Act
        var result = await _tenantService.DeleteTenantAsync(tenantId, deletedBy);

        // Assert
        Assert.True(result);
        _mockTenantRepository.Verify(x => x.DeleteTenantAsync(tenantId, deletedBy), Times.Once);
        _mockUserService.Verify(x => x.DeleteTenantUsersAsync(tenantId, deletedBy), Times.Once);
    }

    [Fact]
    public async Task UpdateTenantAsync_WhenCalled_UpdatesTenantAndReturnsTrue()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var existingTenant = new TenantDto
        {
            TenantId = tenantId,
            TenantName = "Old Name",
            TenantAddress1 = "Old Address"
        };

        var updateRequest = new UpdateTenantRequest
        {
            TenantName = "New Name",
            TenantAddress1 = "New Address",
            TenantCity = "New City",
            TenantState = State.NY,
            TenantZip = "12345",
            ContactFirstName = "John",
            ContactLastName = "Doe",
            ContactEmail = "john@test.com"
        };

        _mockTenantRepository.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(existingTenant);
        _mockTenantRepository.Setup(x => x.UpdateTenantAsync(It.IsAny<TenantDto>()))
            .ReturnsAsync(true);

        // Act
        var result = await _tenantService.UpdateTenantAsync(updateRequest, tenantId);

        // Assert
        Assert.True(result);
        _mockTenantRepository.Verify(x => x.GetTenantByIdAsync(tenantId), Times.Once);
        _mockTenantRepository.Verify(x => x.UpdateTenantAsync(It.Is<TenantDto>(t =>
            t.TenantName == "New Name" &&
            t.TenantAddress1 == "New Address" &&
            t.TenantCity == "New City"
        )), Times.Once);
    }
}
