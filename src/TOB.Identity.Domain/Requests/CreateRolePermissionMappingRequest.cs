using System.Collections.Generic;
using System;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Domain.Requests;

public class CreateRolePermissionMappingRequest
{
    public CreateRolePermissionMappingRequest()
    {
        Permissions = new List<PermissionDto>();
    }

    public Guid RoleId { get; set; }
    public Guid TenantId { get; set; }
    public List<PermissionDto> Permissions { get; set; }
}
