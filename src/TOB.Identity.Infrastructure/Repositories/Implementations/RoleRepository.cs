using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Data;
using RoleEntity = TOB.Identity.Infrastructure.Data.Entities.Role;

namespace TOB.Identity.Infrastructure.Repositories.Implementations;

public class RoleRepository : IRoleRepository
{
    private readonly IdentityDBContext _identityDBContext;
    private readonly IMapper _mapper;

    public RoleRepository(IdentityDBContext identityDBContext, IMapper mapper)
    {
        _identityDBContext = identityDBContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Get All Roles.
    /// </summary>
    /// <returns>list of Role.</returns>
    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roleEntities = await _identityDBContext.Roles
            .Include(r => r.RolePermissionMappings)
                .ThenInclude(rpm => rpm.Permission)
            .OrderBy(u => u.RoleName)
            .ToListAsync();

        return _mapper.Map<IEnumerable<RoleDto>>(roleEntities);
    }

    /// <summary>
    /// Create role.
    /// </summary>
    /// <param name="role">role.</param>
    /// <returns>Returns boolean, whether role is created or not.</returns>
    public async Task<bool> CreateRoleAsync(RoleDto roleDto, Guid createdBy)
    {
        var roleEntity = _mapper.Map<RoleEntity>(roleDto);
        roleEntity.CreatedBy = createdBy;
        roleEntity.CreatedDateTime = DateTime.UtcNow;

        _identityDBContext.Roles.Add(roleEntity);

        return await _identityDBContext.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Update role.
    /// </summary>
    /// <param name="role">role.</param>
    /// <returns>Returns boolean, whether role is updated or not.</returns>
    public async Task<bool> UpdateRoleAsync(RoleDto roleDto, Guid updatedBy)
    {
        var roleEntity = await _identityDBContext.Roles.FirstOrDefaultAsync(u => u.RoleId == roleDto.RoleId);

        roleEntity.RoleName = roleDto.RoleName;
        roleEntity.UpdatedBy = updatedBy;
        roleEntity.UpdatedDateTime = DateTime.UtcNow;

        return await _identityDBContext.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Delete role.
    /// </summary>
    /// <param name="role">role.</param>
    /// <returns>Returns boolean, whether role is deleted or not.</returns>
    public async Task<bool> DeleteRoleAsync(Guid roleId, Guid deletedBy)
    {
        var roleEntity = await _identityDBContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);
        _identityDBContext.Entry(roleEntity).State = EntityState.Deleted;

        return await _identityDBContext.SaveChangesAsync() > 0;
    }

    /// <summary>
    /// Get Role By Id.
    /// </summary>
    /// <param name="roleId">roleId.</param>
    /// <returns>role.</returns>
    public async Task<RoleDto> GetRoleByIdAsync(Guid roleId)
    {
        var roleEntity = await _identityDBContext.Roles
            .Include(r => r.RolePermissionMappings)
                .ThenInclude(rpm => _identityDBContext.Permissions
                    .Where(p => p.PermissionId == rpm.PermissionId))
            .SingleOrDefaultAsync(x => x.RoleId == roleId);

        return _mapper.Map<RoleDto>(roleEntity);
    }
}
