using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Infrastructure.Repositories;

public interface IUserRoleRepository
{
    Task<bool> CreateUserRoleMappingAsync(IEnumerable<UserRoleMappingDto> userRoleMappings);
    Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(Guid userId);

}
