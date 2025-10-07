using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto> CreateRoleAsync(RoleDto roleDto, Guid createdBy);
    Task<bool> UpdateRoleAsync(RoleDto roleDto, Guid updatedBy);
    Task<bool> DeleteRoleAsync(Guid roleId, Guid deletedBy);
    Task<RoleDto> GetRoleByIdAsync(Guid roleId);
    Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(Guid roleId);
    Task<bool> CreateRolePermissionMappingAsync(Guid createdBy, CreateRolePermissionMappingRequest createRoleRightMappingRequest);
}
