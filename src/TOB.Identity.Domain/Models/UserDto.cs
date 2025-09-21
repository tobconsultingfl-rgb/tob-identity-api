using System;
using System.Collections.Generic;

namespace TOB.Identity.Domain.Models;

public class UserDto
{
    public Guid? UserId { get; set; }
    public Guid? ManagerId { get; set; }
    public string ManagerName { get; set; }
    public string ManagerEmail { get; set; }
    public Guid TenantId { get; set; }
    public string TenantName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Company { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public string Fax { get; set; }
    public string MobilePhone { get; set; }
    public string Address1 { get; set; }
    public string Address2 { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string Zip { get; set; }
    public string RoleName { get; set; }
    public List<RoleDto> Roles { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public DateTime? LastLogin { get; set; }        
    public bool IsActive { get; set; }
}
