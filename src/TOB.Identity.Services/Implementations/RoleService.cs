using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Repositories;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services.Implementations;

public class RoleService : IRoleService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IRolePermissionsRepository _rolePermissionsRepository;
    public RoleService(IRoleRepository roleRepository, IRolePermissionsRepository rolePermissionsRepository)
    {
        _roleRepository = roleRepository;
        _rolePermissionsRepository = rolePermissionsRepository;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        return await _roleRepository.GetAllRolesAsync();
    }

    public async Task<RoleDto> CreateRoleAsync(RoleDto roleDto, Guid createdBy)
    {

        roleDto.RoleId = Guid.NewGuid();

        bool moduleStatus = await _roleRepository.CreateRoleAsync(roleDto, createdBy);
        var createdRole = await _roleRepository.GetRoleByIdAsync(roleDto.RoleId.Value);

        return createdRole;
    }

    public async Task<RoleDto> GetRoleByIdAsync(Guid roleId)
    {
        return await _roleRepository.GetRoleByIdAsync(roleId);
    }

    public async Task<bool> UpdateRoleAsync(RoleDto roleDto, Guid updatedBy)
    {
        var result = await _roleRepository.UpdateRoleAsync(roleDto, updatedBy);

        return result;
    }

    public async Task<bool> DeleteRoleAsync(Guid roleId, Guid deletedBy)
    {
        return await _roleRepository.DeleteRoleAsync(roleId, deletedBy);
    }

    public async Task<IEnumerable<PermissionDto>> GetRolePermissionsAsync(Guid tenantId, Guid roleId)
    {
        return await _rolePermissionsRepository.GetRolePermissionsAsync(tenantId, roleId);
    }

    public async Task<bool> CreateRolePermissionMappingAsync(Guid createdBy, CreateRolePermissionMappingRequest createRolePermissionMappingRequest)
    {
        var roleId = createRolePermissionMappingRequest.RoleId;
        var tenantId = createRolePermissionMappingRequest.TenantId;
        var roleMappingList = new List<RolePermissionMappingDto>();

        foreach (var right in createRolePermissionMappingRequest.Permissions)
        {
            var roleRightMappingDTO = new RolePermissionMappingDto { Id = Guid.NewGuid(), TenantId = tenantId, RoleId = createRolePermissionMappingRequest.RoleId, PermissionId = right.PermissionId.Value, CreatedBy = createdBy, CreatedDateTime = DateTime.UtcNow, UpdatedBy = createdBy, UpdatedDateTime = DateTime.UtcNow };

            roleMappingList.Add(roleRightMappingDTO);
        }

        var results = await _rolePermissionsRepository.CreateRolePermissionsMappingAsync(roleMappingList);

        return results;
    }
}
