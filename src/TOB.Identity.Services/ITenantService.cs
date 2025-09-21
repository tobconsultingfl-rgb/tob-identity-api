using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services;

public interface ITenantService
{
    Task<IEnumerable<TenantDto>> GetTenantsByUserIdAsync(Guid tenantId, Guid userId);
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync(Guid tenantId);
    Task<TenantDto> CreateTenantAsync(CreateTenantRequest createTenantRequest, Guid createdBy);
    Task<TenantDto> GetTenantByIdAsync(Guid tenantId);
    Task<bool> UpdateTenantAsync(UpdateTenantRequest request, Guid tenantId);
    Task<bool> DeleteTenantAsync(Guid tenantId, Guid deletedBy);
}
