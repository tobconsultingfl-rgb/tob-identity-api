
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TOB.Identity.Domain.Models;

namespace TOB.Identity.Domain.Requests;

public class CreateUserRequest
{
    [Required]
    public Guid TenantId { get; set; }

    public Guid? ManagerId { get; set; }
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }

    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string Email { get; set; }
    [Required]
    public string MobilePhone { get; set; }
    public List<RoleDto> Roles { get; set; }
}
