using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using System.Security.Claims;
using TOB.Identity.API.Controllers;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Services;
using Xunit;

namespace TOB.Identity.API.Test;

public class TenantsControllerTests
{
    private readonly Mock<ITenantService> _mockTenantService;
    private readonly Mock<IMemoryCache> _mockMemoryCache;
    private readonly TenantsController _controller;
    private readonly Guid _testUserId;

    public TenantsControllerTests()
    {
        _mockTenantService = new Mock<ITenantService>();
        _mockMemoryCache = new Mock<IMemoryCache>();
        _testUserId = Guid.NewGuid();

        _controller = new TenantsController(_mockTenantService.Object, _mockMemoryCache.Object);

        // Setup controller context with user claims
        var claims = new List<Claim>
        {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", _testUserId.ToString()),
            new Claim("extension_TenantId", Guid.NewGuid().ToString()),
            new Claim("extension_Roles", "admin")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    [Fact]
    public async Task GetAllTenantsAsync_WithoutCache_ReturnsTenants()
    {
        // Arrange
        var tenants = new List<TenantDto>
        {
            new TenantDto { TenantId = Guid.NewGuid(), TenantName = "Tenant 1" },
            new TenantDto { TenantId = Guid.NewGuid(), TenantName = "Tenant 2" }
        };

        object cachedValue = null;
        _mockMemoryCache.Setup(x => x.TryGetValue(It.IsAny<object>(), out cachedValue))
            .Returns(false);

        _mockTenantService.Setup(x => x.GetAllTenantsAsync())
            .ReturnsAsync(tenants);

        var cacheEntry = Mock.Of<ICacheEntry>();
        _mockMemoryCache.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns(cacheEntry);

        // Act
        var result = await _controller.GetAllTenantsAsync();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTenants = Assert.IsAssignableFrom<IEnumerable<TenantDto>>(okResult.Value);
        Assert.Equal(2, returnedTenants.Count());
        _mockTenantService.Verify(x => x.GetAllTenantsAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTenantById_WhenTenantExists_ReturnsTenant()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new TenantDto { TenantId = tenantId, TenantName = "Test Tenant" };

        _mockTenantService.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(tenant);

        // Act
        var result = await _controller.GetTenantById(tenantId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedTenant = Assert.IsType<TenantDto>(okResult.Value);
        Assert.Equal(tenantId, returnedTenant.TenantId);
        Assert.Equal("Test Tenant", returnedTenant.TenantName);
    }

    [Fact]
    public async Task GetTenantById_WhenTenantDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        _mockTenantService.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync((TenantDto)null);

        // Act
        var result = await _controller.GetTenantById(tenantId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task CreateTenantAsync_WithValidRequest_ReturnsCreated()
    {
        // Arrange
        var request = new CreateTenantRequest
        {
            TenantName = "New Tenant",
            ContactFirstName = "John",
            ContactLastName = "Doe",
            ContactEmail = "john@test.com"
        };

        var createdTenant = new TenantDto
        {
            TenantId = Guid.NewGuid(),
            TenantName = "New Tenant"
        };

        _mockTenantService.Setup(x => x.CreateTenantAsync(request, _testUserId))
            .ReturnsAsync(createdTenant);

        // Act
        var result = await _controller.CreateTenantAsync(request);

        // Assert
        var createdResult = Assert.IsType<CreatedResult>(result);
        var returnedTenant = Assert.IsType<TenantDto>(createdResult.Value);
        Assert.Equal("New Tenant", returnedTenant.TenantName);
        _mockTenantService.Verify(x => x.CreateTenantAsync(request, It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task UpdateTenantAsync_WhenTenantExists_ReturnsNoContent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new TenantDto { TenantId = tenantId, TenantName = "Existing Tenant" };
        var updateRequest = new UpdateTenantRequest { TenantName = "Updated Tenant" };

        _mockTenantService.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(tenant);
        _mockTenantService.Setup(x => x.UpdateTenantAsync(updateRequest, tenantId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.UpdateTenantAsync(tenantId, updateRequest);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockTenantService.Verify(x => x.UpdateTenantAsync(updateRequest, tenantId), Times.Once);
    }

    [Fact]
    public async Task UpdateTenantAsync_WhenTenantDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var updateRequest = new UpdateTenantRequest { TenantName = "Updated Tenant" };

        _mockTenantService.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync((TenantDto)null);

        // Act
        var result = await _controller.UpdateTenantAsync(tenantId, updateRequest);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockTenantService.Verify(x => x.UpdateTenantAsync(It.IsAny<UpdateTenantRequest>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task DeleteTenantAsync_WhenTenantExists_ReturnsNoContent()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var tenant = new TenantDto { TenantId = tenantId, TenantName = "Test Tenant" };

        _mockTenantService.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync(tenant);
        _mockTenantService.Setup(x => x.DeleteTenantAsync(tenantId, It.IsAny<Guid>()))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteTenantAsync(tenantId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockTenantService.Verify(x => x.DeleteTenantAsync(tenantId, It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task DeleteTenantAsync_WhenTenantDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var tenantId = Guid.NewGuid();

        _mockTenantService.Setup(x => x.GetTenantByIdAsync(tenantId))
            .ReturnsAsync((TenantDto)null);

        // Act
        var result = await _controller.DeleteTenantAsync(tenantId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockTenantService.Verify(x => x.DeleteTenantAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }
}
