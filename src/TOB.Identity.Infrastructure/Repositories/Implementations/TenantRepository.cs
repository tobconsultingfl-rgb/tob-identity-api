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

public class TenantRepository : ITenantRepository
{
    private readonly IdentityDBContext _pcMainDBContext;
    private readonly IMapper _mapper;

    public TenantRepository(IdentityDBContext pcMasterDBConext, IMapper mapper)
    {
        _pcMainDBContext = pcMasterDBConext;
        _pcMainDBContext.Database.EnsureCreated();
        _mapper = mapper;
    }

    public async Task<bool> DoesTenantExistsAsync(string TenantName)
    {
        var Tenant = await _pcMainDBContext.Tenants.FirstOrDefaultAsync(x => x.TenantName.ToLower().Trim() == TenantName.ToLower().Trim());

        return Tenant != null;
    }

    public async Task<TenantDto> CreateTenantAsync(TenantDto TenantDTO, Guid CreatedbyId)
    {
        TenantDTO.CreatedDateTime = DateTime.UtcNow;
        TenantDTO.UpdatedDateTime = DateTime.UtcNow;

        var tenantEntity = _mapper.Map<Tenant>(TenantDTO);

        await _pcMainDBContext.Tenants.AddAsync(tenantEntity);
        await _pcMainDBContext.SaveChangesAsync();

        return TenantDTO; 
    }

    public async Task<IEnumerable<TenantDto>> GetAllTenantsAsync()
    {
        var Tenants = await _pcMainDBContext.Tenants.ToListAsync();
        return _mapper.Map<List<TenantDto>>(Tenants);
    }

    public async Task<bool> DeleteTenantAsync(Guid tenantId, Guid deletedBy )
    {
        var tenantEntity = await _pcMainDBContext.Tenants.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        tenantEntity.IsActive = false;
        tenantEntity.UpdatedDateTime = DateTime.UtcNow;
        tenantEntity.UpdatedBy = deletedBy;
        
        _pcMainDBContext.Entry(tenantEntity).State = EntityState.Modified;
        await _pcMainDBContext.SaveChangesAsync();
        
        return true;
    }

    public async Task<TenantDto> GetTenantByIdAsync(Guid tenantId)
    {
        var Tenant = await _pcMainDBContext.Tenants.FirstOrDefaultAsync(x => x.TenantId == tenantId);
        return _mapper.Map<TenantDto>(Tenant);
    }

    public async Task<IEnumerable<TenantDto>> GetTenantsByUserAsync(Guid tenantId, Guid userId)
    {
        var Tenants = await (from t in _pcMainDBContext.Tenants
                             join u in _pcMainDBContext.Users on t.TenantId equals u.TenantId
                             where u.UserId == userId
                             orderby t.TenantName
                             select t).ToListAsync();

        return _mapper.Map<List<TenantDto>>(Tenants);
    }


    public async Task<bool> UpdateTenantAsync(TenantDto TenantDTO)
    {
        var tenantEntity = _mapper.Map<Tenant>(TenantDTO);
        tenantEntity.UpdatedDateTime = DateTime.UtcNow;

        _pcMainDBContext.Entry(tenantEntity).State = EntityState.Modified;
        await _pcMainDBContext.SaveChangesAsync();

        return true;
    }
}
