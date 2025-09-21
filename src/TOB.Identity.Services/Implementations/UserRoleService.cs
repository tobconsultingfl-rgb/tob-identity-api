
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Repositories;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services.Implementations;

public class UserRoleService : IUserRoleService
{
    private readonly IUserRoleRepository _userRoleRepository;

    public UserRoleService(IUserRoleRepository userRoleRepository)
    {
        _userRoleRepository = userRoleRepository;
    }

    public async Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(Guid userId)
    {
        return await _userRoleRepository.GetRolesByUserIdAsync(userId);
    }
    public async Task<bool> AssignUserToRoleAsync(CreateUserRoleRequest createUserRoleRequest, Guid createdBy)
    {
        var result = await _userRoleRepository.CreateUserRoleMappingAsync(new List<UserRoleMappingDto> { new UserRoleMappingDto { Id = Guid.NewGuid(), TenantId = createUserRoleRequest.TenantId, UserId = createUserRoleRequest.UserId, RoleId = createUserRoleRequest.RoleId, CreatedBy = createdBy, CreatedDateTime = DateTime.UtcNow } });

        return result;
    }
}
