using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Infrastructure.Repositories;

public interface IRolePermissionsRepository
{
    Task<bool> CreateRolePermissionsMappingAsync(List<RolePermissionMappingDto> roleRightMappings);

    Task<List<PermissionDto>> GetRolePermissionsAsync(Guid tenantId, Guid roleId);
}
