using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;
using TOB.Identity.Domain.Models.Requests;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services;

public interface IUserService
{
    Task<UserDto> GetUserByIdAsync(Guid userId);
    Task<IEnumerable<UserDto>> GetAllUsersByTenantIdAsync(Guid licenseeId);
    Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, Guid createdByUserId);
    Task<bool> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid updatedByUserId);
    Task<bool> DeleteUserAsync(Guid userId, Guid deletedByUserId);
    Task<bool> DeleteTenantUsersAsync(Guid licenseeId, Guid deletedByUserId);
    Task<bool> DoesUsernameExistsAsync(string userName);
}
