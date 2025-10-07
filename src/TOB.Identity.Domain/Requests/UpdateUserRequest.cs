using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TOB.Identity.Domain.Models.Requests;

public class UpdateUserRequest
{
    [Required]
    public Guid UserId { get; set; }
    [Required]
    public Guid TenantId { get; set; }
    public Guid? ManagerId { get; set; }

    [Required]
    public string FirstName { get; set; }
    [Required]
    public string LastName { get; set; }
    [Required]
    public string MobilePhone { get; set; }
    public long? MaxQuoteAmount { get; set; }
    public List<RoleDto> Roles { get; set; }
}
