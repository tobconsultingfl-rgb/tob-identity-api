using AutoMapper;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using TOB.Identity.Domain.AppSettings;
using TOB.Identity.Domain.Models;

using TOB.Identity.Domain.Requests;
using TOB.Identity.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models.Requests;
using Microsoft.Graph.Models;

namespace TOB.Identity.Services.Implementations;

public class UserService : IUserService
{
    private readonly GraphServiceClient _graphServiceClient;
    private readonly AzureAd _azureAdConfig;
    private readonly IUserRepository _userRepository;
    private readonly IUserRoleRepository _roleMappingRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IUserRoleRepository roleMappingRepository, IOptions<AzureAd> azureAdConfig, GraphServiceClient graphServiceClient, IMapper mapper)
    {
        _userRepository = userRepository;
        _roleMappingRepository = roleMappingRepository;
        _azureAdConfig = azureAdConfig.Value;
        _graphServiceClient = graphServiceClient;
        _mapper = mapper;
    }

    public async Task<UserDto> CreateUserAsync(CreateUserRequest createUserRequest, Guid createdByUserId)
    {
        var userDto = _mapper.Map<UserDto>(createUserRequest);

        if (createUserRequest.ManagerId.HasValue)
        {
            userDto.ManagerId = createUserRequest.ManagerId.Value;
        }

        // Create the User account in B2C first.
        var adUser = await CreateADUserAsync(userDto, createUserRequest.Password);
        userDto.UserId = new Guid(adUser.Id);
        userDto.CreatedBy = createdByUserId;
        userDto.UpdatedBy = createdByUserId;

        var createdUser = await _userRepository.CreateUserAsync(userDto);

        var roleMappings = GetUserRoleMappings(userDto);

        await _roleMappingRepository.CreateUserRoleMappingAsync(roleMappings);

        return await GetUserByIdAsync(createdUser.UserId.Value);
    }

    public async Task<bool> DeleteTenantUsersAsync(Guid tenantId, Guid deletedByUserId)
    {
        var userDTOs = await _userRepository.GetTenantUsersAsync(tenantId);

        foreach (var userDTO in userDTOs)
        {
            var wasDeleted = await DeactivateADUserAsync(userDTO.UserId.ToString());
        }

        return await _userRepository.DeleteTenantUsersAsync(tenantId, deletedByUserId);
    }

    public async Task<bool> DeleteUserAsync(Guid userId, Guid deletedByUserId)
    {
        var userDto = await _userRepository.GetUserByIdAsync(userId);
        var deletedFromAd = await DeactivateADUserAsync(userDto.UserId.ToString());

        return await _userRepository.DeleteUserAsync(userId, deletedByUserId);
    }

    public async Task<bool> DoesUsernameExistsAsync(string userName)
    {
        return await _userRepository.DoesUsernameExistsAsync(userName);
    }

    public async Task<IEnumerable<UserDto>> GetAllUsersByTenantIdAsync(Guid tenantId)
    {
        var users = await _userRepository.GetTenantUsersAsync(tenantId);

        foreach (var user in users)
        {
            await AddManagerInfoAsync(user);

            var usersRoles = await _roleMappingRepository.GetRolesByUserIdAsync(user.UserId.Value);

            var primaryRoleId = usersRoles.FirstOrDefault().RoleId;
            var roleName = await _userRepository.GetRoleNameAsync(primaryRoleId.Value);
            var TenantName = await _userRepository.GetTenantNameAsync(user.TenantId);

            user.TenantName = TenantName;
            user.RoleName = roleName;

            user.Roles = usersRoles.ToList();
        }

        return users;
    }

    public async Task<UserDto> GetUserByIdAsync(Guid userId)
    {
        var userDto = await _userRepository.GetUserByIdAsync(userId);

        await AddManagerInfoAsync(userDto);

        var usersRoles = await _roleMappingRepository.GetRolesByUserIdAsync(userId);

        var primaryRoleId = usersRoles.FirstOrDefault().RoleId;
        var roleName = await _userRepository.GetRoleNameAsync(primaryRoleId.Value);
        var TenantName = await _userRepository.GetTenantNameAsync(userDto.TenantId);

        userDto.TenantName = TenantName;
        userDto.RoleName = roleName;

        userDto.Roles = usersRoles.ToList();

        return userDto;
    }

    public async Task<bool> UpdateUserAsync(UpdateUserRequest updateUserRequest, Guid updatedByUserId)
    {
        var userDto = _mapper.Map<UserDto>(updateUserRequest);
        userDto.UpdatedBy = updatedByUserId;
        userDto.UpdatedDateTime = DateTime.UtcNow;

        var adUser = await UpdateADUserAsync(userDto);

        var roleMappings = GetUserRoleMappings(userDto);
        
        await _roleMappingRepository.CreateUserRoleMappingAsync(roleMappings);

        return await _userRepository.UpdateUserAsync(userDto);
    }

    private async Task AddManagerInfoAsync(UserDto userDto)
    {
        if (userDto.ManagerId.HasValue)
        {
            var manager = await _userRepository.GetUserByIdAsync(userDto.ManagerId.Value);

            if (manager != null)
            {
                userDto.ManagerName = $"{manager.FirstName} {manager.LastName}";
                userDto.ManagerEmail = manager.Email;
            }
        }
    }

    private async Task<User> GetADUserByIdAsync(Guid userId)
    {
        var user = await _graphServiceClient
                            .Users[userId.ToString()]
                            .GetAsync();
        return user;
    }

    /// <summary>
    /// Create ADUser.
    /// </summary>
    /// <param name="userDto">user.</param>
    /// <returns>ADUser.</returns>
    private async Task<User> CreateADUserAsync(UserDto userDto, string passWord)
    {
        var userADObject = new User
        {
            AccountEnabled = true,
            GivenName = userDto.FirstName,
            Surname = userDto.LastName,
            MobilePhone = userDto.MobilePhone,
            DisplayName = $"{userDto.FirstName} {userDto.LastName}",
            Identities = new List<ObjectIdentity>
            {
                new ObjectIdentity
                {
                    SignInType = "EmailAddress",
                    Issuer = _azureAdConfig.Domain,
                    IssuerAssignedId = userDto.Email,
                },
            },
            PasswordProfile = new PasswordProfile
            {
                ForceChangePasswordNextSignIn = false,
                Password = passWord,
            },
            PasswordPolicies = "DisableStrongPassword",
            Mail = userDto.Email
        };

        User manager = null;

        if (userDto.ManagerId.HasValue)
        {
            manager = await GetADUserByIdAsync(userDto.ManagerId.Value);

            userADObject.Manager = manager;
        }

        var extensionCustomAttributeData = new Dictionary<string, object>();
        extensionCustomAttributeData.Add(GetCompleteAttributeName("TenantId"), userDto.TenantId.ToString());
        extensionCustomAttributeData.Add(GetCompleteAttributeName("Roles"), GetCommaDelimitedRoles(userDto.Roles));

        userADObject.AdditionalData = extensionCustomAttributeData;


        var addedUser = await _graphServiceClient.Users.PostAsync(userADObject);

        await _graphServiceClient.Users[addedUser.Id].GetAsync();

        return addedUser;
    }

    private async Task<User> UpdateADUserAsync(UserDto userDto)
    {
        var adUser = await _graphServiceClient
            .Users[userDto.UserId.ToString()]
            .GetAsync();

        adUser.GivenName = userDto.FirstName;
        adUser.Surname = userDto.LastName;
        adUser.MobilePhone = userDto.MobilePhone;
        adUser.DisplayName = $"{userDto.FirstName} {userDto.LastName}";

        var extensionCustomAttributeData = new Dictionary<string, object>();
        extensionCustomAttributeData.Add(GetCompleteAttributeName("TenantId"), userDto.TenantId.ToString());
        extensionCustomAttributeData.Add(GetCompleteAttributeName("Roles"), GetCommaDelimitedRoles(userDto.Roles));

        adUser.AdditionalData = extensionCustomAttributeData;

        await _graphServiceClient.Users[adUser.Id].PatchAsync(adUser);

        return adUser;
    }

    private static string GetCommaDelimitedRoles(List<RoleDto> roleDTOs)
    {
        var roleList = roleDTOs.Select(x => x.RoleName.ToLower().Replace(' ', '_')).ToList();

        return string.Join(",", roleList);
    }

    private static List<UserRoleMappingDto> GetUserRoleMappings(UserDto userDto)
    {
        var mappings = new List<UserRoleMappingDto>();
        var userId = userDto.UserId;
        var createdBy = userDto.CreatedBy;

        foreach (var role in userDto.Roles)
        {
            mappings.Add(new UserRoleMappingDto { UserId = userId.Value, RoleId = role.RoleId.Value, CreatedBy = createdBy, CreatedDateTime = DateTime.UtcNow });
        }

        return mappings;
    }
    private async Task<bool> DeactivateADUserAsync(string adUserId)
    {
        var user = await _graphServiceClient
            .Users[adUserId]
            .GetAsync();

        user.AccountEnabled = false;

        await _graphServiceClient.Users[adUserId].PatchAsync(user);

        return true;
    }

    /// <summary>
    /// Gets the atribute name prefixed with client id of B2C.
    /// </summary>
    /// <param name="attributeName">B2C attribute name.</param>
    /// <returns>Complete B2C attribute name.</returns>
    private string GetCompleteAttributeName(string attributeName)
    {
        var adExtensionId = _azureAdConfig.ExtensionId.Replace("-", string.Empty);

        if (string.IsNullOrWhiteSpace(attributeName))
        {
            throw new ArgumentException("Parameter cannot be null", nameof(attributeName));
        }

        return $"extension_{adExtensionId}_{attributeName}";
    }
}
