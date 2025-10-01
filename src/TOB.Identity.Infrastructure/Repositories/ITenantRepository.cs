using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Infrastructure.Repositories;

public interface ITenantRepository
{
    Task<bool> DoesTenantExistsAsync(string tenantName);
    Task<IEnumerable<TenantDto>> GetTenantsByUserAsync(Guid tenantId, Guid userId);
    Task<IEnumerable<TenantDto>> GetAllTenantsAsync();
    Task<TenantDto> CreateTenantAsync(TenantDto TenantDTO, Guid createdbyId);
    Task<bool> UpdateTenantAsync(TenantDto tenantDTO);
    Task<TenantDto> GetTenantByIdAsync(Guid tenantId);
    Task<bool> DeleteTenantAsync(Guid tenantId, Guid deletedBy);
}
