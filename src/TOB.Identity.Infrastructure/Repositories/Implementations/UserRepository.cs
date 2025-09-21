using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Data;
using UserEntity = TOB.Identity.Infrastructure.Data.Entities.User;

namespace TOB.Identity.Infrastructure.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly IdentityDBContext _identityDBContext;
    private readonly IMapper _mapper;

    public UserRepository(IdentityDBContext identityDBConext, IMapper mapper)
    {
        _identityDBContext = identityDBConext;
        _identityDBContext.Database.EnsureCreated();
        _mapper = mapper; 
    }

    public async Task<bool> AddUserAsOwnerToADGroupAsync(TenantDto tenantDto, Guid createdbyId)
    {
        var userToBeCreated = new UserDto
        {
            FirstName = tenantDto.ContactFirstName,
            LastName = tenantDto.ContactLastName,
            Email = tenantDto.ContactEmail,
            MobilePhone = tenantDto.ContactPhoneNumber,
            TenantId = tenantDto.TenantId,
            Username = tenantDto.ContactEmail,
            UserId = Guid.NewGuid(),
            CreatedBy = createdbyId,
            UpdatedBy = createdbyId,
            CreatedDateTime = DateTime.UtcNow,
            UpdatedDateTime = DateTime.UtcNow
        };

        var newUser = await CreateUserAsync(userToBeCreated);

        return newUser != null;
    }

    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        var userEntity = _mapper.Map<UserEntity>(userDto);

        _identityDBContext.Users.Add(userEntity);

        await _identityDBContext.SaveChangesAsync();
        
        return _mapper.Map<UserDto>(userEntity);
    }

    public async Task<bool> DeleteTenantUsersAsync(Guid TenantId, Guid deletedByUserId)
    {
        var userEntities = await _identityDBContext.Users.Where(x => x.TenantId == TenantId).ToListAsync();

        foreach (var userEntity in userEntities)
        {
            userEntity.IsActive = false;
            userEntity.UpdatedBy = deletedByUserId;
        }
        
        return await _identityDBContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteUserAsync(Guid userId, Guid deletedByUserId)
    {
        var userEntity = await _identityDBContext.Users.FirstOrDefaultAsync(x => x.UserId == userId);

        userEntity.IsActive = false;
        userEntity .UpdatedBy = deletedByUserId;

        return await _identityDBContext.SaveChangesAsync() > 0;
    }

    public async Task<bool> DoesUsernameExistsAsync(string username)
    {
        var userEntity = await _identityDBContext.Users.Where(x => x.Username == username).ToListAsync();

        return userEntity != null && userEntity.Any();
    }
     

    public async Task<IEnumerable<UserDto>> GetTenantUsersAsync(Guid TenantId)
    {
        var userEntities = await _identityDBContext.Users
                                                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                                                .Where(x => x.TenantId == TenantId && x.IsActive == true)
                                                .ToListAsync();

        return _mapper.Map<List<UserDto>>(userEntities);
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var userEntity = await _identityDBContext.Users.SingleOrDefaultAsync(u => u.UserId == userId);

        return _mapper.Map<UserDto>(userEntity);
    } 
    public async Task<UserDto> GetUserByUsernameAsync(string userName)
    {
        var userEntity = await _identityDBContext.Users.SingleOrDefaultAsync(u => u.Username == userName);

        return _mapper.Map<UserDto>(userEntity);
    }

    public async Task<bool> UpdateUserAsync(UserDto UserDto)
    {
        var userEntity = await _identityDBContext.Users.SingleAsync(u => u.UserId == UserDto.UserId);

        userEntity.TenantId = UserDto.TenantId;
        userEntity.ManagerId = UserDto.ManagerId;
        userEntity.FirstName = UserDto.FirstName;
        userEntity.LastName = UserDto.LastName;
        userEntity.MobilePhone = UserDto.MobilePhone;
        
        _identityDBContext.Users.Update(userEntity);

        return await _identityDBContext.SaveChangesAsync() > 0;
    }


    public async Task<string> GetRoleNameAsync(Guid roleId)
    {
        var role = await _identityDBContext.Roles.FirstOrDefaultAsync(r => r.RoleId == roleId);

        return role.RoleName;
    }

    public async Task<string> GetTenantNameAsync(Guid TenantId)
    {
        var Tenant = await _identityDBContext.Tenants.FirstOrDefaultAsync(r => r.TenantId == TenantId);

        return Tenant.TenantName;
    }        
}
