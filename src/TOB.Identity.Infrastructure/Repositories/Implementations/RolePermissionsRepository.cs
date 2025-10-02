using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Infrastructure.Data;
using System.Linq;
using TOB.Identity.Domain.Models;
using Microsoft.EntityFrameworkCore;
using TOB.Identity.Infrastructure.Data.Entities;

namespace TOB.Identity.Infrastructure.Repositories.Implementations;

public class RolePermissionsRepository : IRolePermissionsRepository
{
    private readonly IdentityDBContext _identityDBContext;
    private readonly IMapper _mapper;

    public RolePermissionsRepository(IdentityDBContext identityDBContext, IMapper mapper)
    {
        _identityDBContext = identityDBContext;
        _identityDBContext.Database.EnsureCreated();
        _mapper = mapper;
    }

    public async Task<bool> CreateRolePermissionsMappingAsync(List<RolePermissionMappingDto> rolePermissionMappings)
    {
        var tenantId = rolePermissionMappings[0].TenantId;
        var roleId = rolePermissionMappings[0].RoleId;

        var roleRightMappingEntities = await _identityDBContext.RolePermissionMappings.Where(x => x.RoleId == roleId).ToListAsync();

        if (roleRightMappingEntities.Count > 0)
        {
            _identityDBContext.RolePermissionMappings.RemoveRange(roleRightMappingEntities);
            await _identityDBContext.SaveChangesAsync();
        }

        var newRolePermissionMappingEntities = _mapper.Map<List<RolePermissionMapping>>(rolePermissionMappings);

        _identityDBContext.RolePermissionMappings.AddRange(newRolePermissionMappingEntities);
        return await _identityDBContext.SaveChangesAsync() > 0;
    }

    public async Task<List<PermissionDto>> GetRolePermissionsAsync(Guid roleId)
    {
        var roleRightMappingEntity = await _identityDBContext.RolePermissionMappings.Where(x => x.RoleId == roleId).ToListAsync();
        var rightIds = roleRightMappingEntity.Select(x => x.PermissionId).ToArray();

        var allPermissions = await _identityDBContext.Permissions.OrderBy(r => r.SortOrder).ToListAsync();

        foreach (var right in allPermissions)
        {
            if (rightIds.Contains(right.PermissionId))
            {
                right.IsActive = true;
            }
            else
            {
                right.IsActive = false;
            }
        }

        return _mapper.Map<List<PermissionDto>>(allPermissions);
    }
}
