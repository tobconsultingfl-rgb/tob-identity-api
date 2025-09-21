using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TOB.Identity.Domain.Models;
using TOB.Identity.Infrastructure.Data;
using TOB.Identity.Infrastructure.Data.Entities;

namespace TOB.Identity.Infrastructure.Repositories.Implementations;

public class UserRoleRepository : IUserRoleRepository
{
    private readonly IdentityDBContext _identityDBContext;
    private readonly IMapper _mapper;

    public UserRoleRepository(IdentityDBContext identityDBContext, IMapper mapper)
    {
        _identityDBContext = identityDBContext;
        _mapper = mapper;
    }

    public async Task<bool> CreateUserRoleMappingAsync(IEnumerable<UserRoleMappingDto> userRoleMappingDtos)
    {            
        var userRoleMappings = await _identityDBContext.UserRoleMappings.Where(x => x.UserId == userRoleMappingDtos.FirstOrDefault().UserId).ToListAsync();

        if (userRoleMappings.Count() > 0)
        {
            _identityDBContext.UserRoleMappings.RemoveRange(userRoleMappings);
            await _identityDBContext.SaveChangesAsync();
        }

        var userRoleMappingEntities = _mapper.Map<IEnumerable<UserRoleMapping>>(userRoleMappingDtos);

        foreach (var userRoleMappingEntity in userRoleMappingEntities)
        {
            userRoleMappingEntity.Id = Guid.NewGuid();
            _identityDBContext.UserRoleMappings.Add(userRoleMappingEntity);
        }
        
        return await _identityDBContext.SaveChangesAsync() > 0;
    }


    public async Task<IEnumerable<RoleDto>> GetRolesByUserIdAsync(Guid userId)
    {            
        var userRoleIDs = await _identityDBContext.UserRoleMappings.Where(x => x.UserId == userId).Select(x => x.RoleId).ToListAsync();

        if (userRoleIDs.Any())
        {
            var roleEntities = await _identityDBContext.Roles.Where(x => userRoleIDs.Contains(x.RoleId)).ToListAsync();

            return _mapper.Map<List<RoleDto>>(roleEntities);
        }

        return null;
    }
}
