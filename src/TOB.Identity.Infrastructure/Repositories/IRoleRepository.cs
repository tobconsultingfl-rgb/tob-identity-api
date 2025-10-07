using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Infrastructure.Repositories;

public interface IRoleRepository
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<bool> CreateRoleAsync(RoleDto RoleDto, Guid createdBy);
    Task<bool> UpdateRoleAsync(RoleDto RoleDto, Guid updatedBy);
    Task<bool> DeleteRoleAsync(Guid roleId, Guid deletedBy);
    Task<RoleDto> GetRoleByIdAsync(Guid roleId);
}
