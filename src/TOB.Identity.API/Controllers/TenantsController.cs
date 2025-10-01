using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using TOB.Identity.Services;
using TOB.Identity.Domain.Models;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.API.Controllers;

[Authorize]
[Route("/tenants")]
[ApiController]
public class TenantsController : BaseController
{
    private readonly ITenantService _tenantService;       
    private IMemoryCache _memoryCache;
    private readonly string cacheKey = "TenantList";

    public TenantsController(ITenantService tenantService, IMemoryCache memoryCache)
    {
        _tenantService = tenantService;
        _memoryCache = memoryCache;
    }

    [HttpPost]
    [SwaggerOperation(
        Summary = "Creates a new Tenant",
        OperationId = "CreateTenant",
        Tags = new[] { "Tenants" }
    )]
    [ProducesResponseType(typeof(TenantDto), 201)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)] 

    public async Task<IActionResult> CreateTenantAsync([FromForm] CreateTenantRequest createTenantRequest)
    {
        var currentUserId = new Guid(CurrentUserId);
        var response = await _tenantService.CreateTenantAsync(createTenantRequest, currentUserId);

        _memoryCache.Remove(cacheKey);

        return Created(string.Empty, response);
    }

    [HttpPut("{tenantId}", Name = "Update Tenant")]
    [SwaggerOperation(
        Summary = "Updates an existing tenant",
        OperationId = "UpdateTenant",
        Tags = new[] { "Tenants" }
    )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    public async Task<IActionResult> UpdateTenantAsync(Guid tenantId, [FromForm] UpdateTenantRequest updateTenantRequest)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound();
        } 

        await _tenantService.UpdateTenantAsync(updateTenantRequest, tenantId);
        
        return NoContent();
    }

    [HttpGet]
    [SwaggerOperation(
        Summary = "Get All Tenants",
        OperationId = "GetAllTenantsAsync",
        Tags = new[] { "Tenants" }
    )]
    [ProducesResponseType(typeof(IEnumerable<TenantDto>), 200)]
    public async Task<IActionResult> GetAllTenantsAsync()
    {
        if (_memoryCache.TryGetValue(cacheKey, out IEnumerable<TenantDto> cachedTenants))
        {
            return Ok(cachedTenants);
        }
        var currentUserId = CurrentUserId;

        var tenants = await _tenantService.GetAllTenantsAsync();

        _memoryCache.Set(cacheKey, tenants);

        return Ok(tenants);
    }


    [HttpGet("{tenantID}", Name = "Get Tenant By ID")]
    [SwaggerOperation(
        Summary = "Get Tenant by ID",
        OperationId = "GetTenantById",
        Tags = new[] { "Tenants" }
    )]
    [ProducesResponseType(typeof(TenantDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetTenantById(Guid tenantID)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(tenantID);

        if (tenant == null)
        {
            return NotFound();
        }

        return Ok(tenant);
    }

    [HttpDelete("{tenantId}", Name = "Delete (deactivates) Tenant")]
    [SwaggerOperation(
        Summary = "Deletes (deactivates) a Tenant",
        OperationId = "DeleteTenantAsync",
        Tags = new[] { "Tenants" }
    )]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(typeof(ValidationProblemDetails), 400)]
    [Consumes("application/json")]
    [Produces("application/json")]
    public async Task<IActionResult> DeleteTenantAsync(Guid tenantId)
    {
        var tenant = await _tenantService.GetTenantByIdAsync(tenantId);

        if (tenant == null)
        {
            return NotFound();
        }

        var result = await _tenantService.DeleteTenantAsync(tenantId, new Guid(CurrentUserId));

        return NoContent();
    }
}
