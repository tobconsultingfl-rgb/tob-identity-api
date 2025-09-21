using AutoMapper;
using TOB.Identity.Domain.Models;
using System;
using Role = TOB.Identity.Infrastructure.Data.Entities.Role;
using TOB.Identity.Domain.Requests;
using TOB.Identity.Infrastructure.Data.Entities;
using TOB.Identity.Domain.Models.Requests;

namespace TOB.Identity.Infrastructure.Mapping;

public class IdentityMappingProfile : Profile
{
    public IdentityMappingProfile()
    {
        CreateMap<CreateTenantRequest, TenantDto>()
            .ForMember(x => x.TenantId, opt => opt.MapFrom(source => Guid.NewGuid()))

            .ForMember(x => x.TenantName, opt => opt.MapFrom(source => source.TenantName))
            .ForMember(x => x.TenantAddress1, opt => opt.MapFrom(source => source.TenantAddress1))
            .ForMember(x => x.TenantAddress2, opt => opt.MapFrom(source => source.TenantAddress2))
            .ForMember(x => x.TenantCity, opt => opt.MapFrom(source => source.TenantCity))
            .ForMember(x => x.TenantState, opt => opt.MapFrom(source => source.TenantState))
            .ForMember(x => x.TenantZip, opt => opt.MapFrom(source => source.TenantZip))
            .ForMember(x => x.TenantPhoneNumber, opt => opt.MapFrom(source => source.TenantPhoneNumber))
            .ForMember(x => x.TenantFax, opt => opt.MapFrom(source => source.TenantFax))
            .ForMember(x => x.ContactFirstName, opt => opt.MapFrom(source => source.ContactFirstName))
            .ForMember(x => x.ContactLastName, opt => opt.MapFrom(source => source.ContactLastName))
            .ForMember(x => x.ContactPhoneNumber, opt => opt.MapFrom(source => source.ContactMobilePhone))
            .ForMember(x => x.ContactEmail, opt => opt.MapFrom(source => source.ContactEmail))
            .ForMember(x => x.CreatedDateTime, opt => opt.MapFrom(source => DateTime.UtcNow))
            .ForMember(x => x.IsActive, opt => opt.MapFrom(source => true));

        CreateMap<TenantDto, Tenant>()
            .ForMember(x => x.TenantId, opt => opt.MapFrom(source => source.TenantId))
            .ForMember(x => x.TenantName, opt => opt.MapFrom(source => source.TenantName))
            .ForMember(x => x.TenantAddress1, opt => opt.MapFrom(source => source.TenantAddress1))
            .ForMember(x => x.TenantAddress2, opt => opt.MapFrom(source => source.TenantAddress2))
            .ForMember(x => x.TenantCity, opt => opt.MapFrom(source => source.TenantCity))
            .ForMember(x => x.TenantState, opt => opt.MapFrom(source => source.TenantState))
            .ForMember(x => x.TenantZip, opt => opt.MapFrom(source => source.TenantZip))
            .ForMember(x => x.TenantPhoneNumber, opt => opt.MapFrom(source => source.TenantPhoneNumber))
            .ForMember(x => x.TenantFax, opt => opt.MapFrom(source => source.TenantFax))
            .ForMember(x => x.ContactFirstName, opt => opt.MapFrom(source => source.ContactFirstName))
            .ForMember(x => x.ContactLastName, opt => opt.MapFrom(source => source.ContactLastName))
            .ForMember(x => x.ContactPhoneNumber, opt => opt.MapFrom(source => source.ContactPhoneNumber))                
            .ForMember(x => x.ContactEmail, opt => opt.MapFrom(source => source.ContactEmail))
            .ForMember(x => x.CreatedDateTime, opt => opt.MapFrom(source => source.CreatedDateTime))
            .ForMember(x => x.UpdatedDateTime, opt => opt.MapFrom(source => source.UpdatedDateTime))
            .ForMember(x => x.IsActive, opt => opt.MapFrom(source => source.IsActive))
            .ForMember(x => x.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
            .ReverseMap();

        CreateMap<RoleDto, Role>().ReverseMap();

        CreateMap<UserDto, User>().ReverseMap();
        CreateMap<UserRoleMappingDto, UserRoleMapping>().ReverseMap(); ;
        
        CreateMap<CreateUserRequest, UserDto>()
            .ForMember(x => x.TenantId, opt => opt.MapFrom(source => source.TenantId))
            .ForMember(x => x.Roles, opt => opt.MapFrom(source => source.Roles))
            .ForMember(x => x.FirstName, opt => opt.MapFrom(source => source.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(source => source.LastName))
            .ForMember(x => x.Email, opt => opt.MapFrom(source => source.Email))
            .ForMember(x => x.Username, opt => opt.MapFrom(source => source.Email))
            .ForMember(x => x.MobilePhone, opt => opt.MapFrom(source => source.MobilePhone))

            .ForMember(x => x.CreatedDateTime, opt => opt.MapFrom(source => DateTime.UtcNow))
            .ForMember(x => x.UpdatedDateTime, opt => opt.MapFrom(source => DateTime.UtcNow))
            .ForMember(x => x.IsActive, opt => opt.MapFrom(source => true));

        CreateMap<UpdateUserRequest, UserDto>()
            .ForMember(x => x.TenantId, opt => opt.MapFrom(source => source.TenantId))
            .ForMember(x => x.Roles, opt => opt.MapFrom(source => source.Roles))
            .ForMember(x => x.FirstName, opt => opt.MapFrom(source => source.FirstName))
            .ForMember(x => x.LastName, opt => opt.MapFrom(source => source.LastName))
            .ForMember(x => x.MobilePhone, opt => opt.MapFrom(source => source.MobilePhone))
            .ForMember(x => x.UpdatedDateTime, opt => opt.MapFrom(source => DateTime.UtcNow))
            .ForMember(x => x.IsActive, opt => opt.MapFrom(source => true));

        CreateMap<RoleDto, Role>().ReverseMap();
        CreateMap<PermissionDto, Permission>().ReverseMap();
        CreateMap<RolePermissionMappingDto, RolePermissionMapping>()
            .ForMember(x => x.Id, opt => opt.MapFrom(source => source.Id))
            .ForMember(x => x.TenantId, opt => opt.MapFrom(source => source.TenantId))
            .ForMember(x => x.RoleId, opt => opt.MapFrom(source => source.RoleId))
            .ForMember(x => x.PermissionId, opt => opt.MapFrom(source => source.PermissionId))
            .ForMember(x => x.CreatedBy, opt => opt.MapFrom(source => source.CreatedBy))
            .ForMember(x => x.UpdatedBy, opt => opt.MapFrom(source => source.UpdatedBy))
            .ForMember(x => x.CreatedDateTime, opt => opt.MapFrom(source => source.CreatedDateTime))
            .ForMember(x => x.UpdatedDateTime, opt => opt.MapFrom(source => source.UpdatedDateTime));

    }

}

