using System;

namespace TOB.Identity.Domain.Requests;

public class CreateUserRoleRequest
{
    public Guid TenantId { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
