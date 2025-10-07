using System;
using System.Collections.Generic;

namespace TOB.Identity.Domain.Models;

public class RoleDto
{
    public Guid? RoleId { get; set; }
    public string RoleName { get; set; }
    public List<PermissionDto> Permissions { get; set; }
}
