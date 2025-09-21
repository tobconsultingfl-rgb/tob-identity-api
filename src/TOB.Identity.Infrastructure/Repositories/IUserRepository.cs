using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Infrastructure.Repositories;

public interface IUserRepository
{
    //Task<IEnumerable<UserDto>> GetAllUsersWithPagingAsync(Guid TenantId, PageRequest pageRequest);
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<UserDto> CreateUserAsync(UserDto UserDto);
    Task<bool> UpdateUserAsync(UserDto UserDto);
    Task<bool> DeleteUserAsync(Guid userId, Guid deletedByUserId);
    Task<bool> DoesUsernameExistsAsync(string username);
    Task<bool> DeleteTenantUsersAsync(Guid TenantId, Guid deletedByUserId);
    Task<bool> AddUserAsOwnerToADGroupAsync(TenantDto tenantDto, Guid createdbyId);
    Task<IEnumerable<UserDto>> GetTenantUsersAsync(Guid TenantId);
    Task<UserDto> GetUserByUsernameAsync(string userName);
    Task<string> GetRoleNameAsync(Guid roleId);
    Task<string> GetTenantNameAsync(Guid TenantId);

}
