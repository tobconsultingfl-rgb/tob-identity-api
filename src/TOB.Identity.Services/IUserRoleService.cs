using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services;

public interface IUserRoleService
{
    Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(Guid userId);
    Task<bool> AssignUserToRoleAsync(CreateUserRoleRequest createUserRoleRequest, Guid createdBy);
}
