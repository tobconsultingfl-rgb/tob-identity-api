using AutoMapper;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOB.Identity.Domain.Requests;

namespace TOB.Identity.Services.Implementations;

public class TenantService : ITenantService
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserService _userService;
    
    private readonly IMapper _mapper;


    public TenantService(ITenantRepository tenantRepository, IRoleRepository roleRepository, IUserService userService, IMapper mapper)
    { 
        _tenantRepository = tenantRepository;
        _roleRepository = roleRepository;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<TenantDto> CreateTenantAsync(CreateTenantRequest createTenantRequest, Guid createdBy)
    {
        var tenantDTO = _mapper.Map<TenantDto>(createTenantRequest);
        tenantDTO.CreatedBy = createdBy;

        var createdTenant = await _tenantRepository.CreateTenantAsync(tenantDTO, createdBy);
        var defaultRoles = await GetDefaultRoleAsync();

        var createUserRequest = new CreateUserRequest { TenantId = createdTenant.TenantId, Roles = defaultRoles.ToList(), FirstName = tenantDTO.ContactFirstName, LastName = tenantDTO.ContactLastName, Email = tenantDTO.ContactEmail, MobilePhone = tenantDTO.ContactPhoneNumber, Password = createTenantRequest.Password };

        await _userService.CreateUserAsync(createUserRequest, createdBy);

        return createdTenant;
    }

    public async Task<bool> DeleteTenantAsync(Guid tenantId, Guid deletedBy)
    {
        await _tenantRepository.DeleteTenantAsync(tenantId, deletedBy);

        await _userService.DeleteTenantUsersAsync(tenantId, deletedBy);

        return true;
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync(Guid tenantId)
    {
        return await _tenantRepository.GetAllTenantsAsync(tenantId);
    }

    public async Task<TenantDto> GetTenantByIdAsync(Guid tenantId)
    {
        return await _tenantRepository.GetTenantByIdAsync(tenantId);
    }

    public async Task<IEnumerable<TenantDto>> GetTenantsByUserIdAsync(Guid tenantId, Guid userId)
    {
        return await _tenantRepository.GetTenantsByUserAsync(tenantId, userId);
    }

    public async Task<bool> UpdateTenantAsync(UpdateTenantRequest request, Guid tenantId)
    {
        var existingTenantDto = await _tenantRepository.GetTenantByIdAsync(tenantId);

        existingTenantDto.TenantName = request.TenantName;
        existingTenantDto.TenantAddress1 = request.TenantAddress1;
        existingTenantDto.TenantAddress2 = request.TenantAddress2;
        existingTenantDto.TenantCity = request.TenantCity;
        existingTenantDto.TenantState = request.TenantState;
        existingTenantDto.TenantZip = request.TenantZip;
        existingTenantDto.ContactFirstName = request.ContactFirstName;
        existingTenantDto.ContactLastName = request.ContactLastName;
        existingTenantDto.ContactEmail = request.ContactEmail;

        return await _tenantRepository.UpdateTenantAsync(existingTenantDto);
    }

    private async Task<IEnumerable<RoleDto>> GetDefaultRoleAsync()
    { 
        var allRoles = await _roleRepository.GetAllRolesAsync();

        var defaultRoles = allRoles.Where(r => r.RoleName.ToLower().Trim() == "administrator").ToList();             

        return defaultRoles;
    }
     
}
