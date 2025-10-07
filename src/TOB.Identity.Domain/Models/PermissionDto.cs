using System; 

namespace TOB.Identity.Domain.Models;

public class PermissionDto
{
    public Guid? PermissionId { get; set; }
    public string PermissionName { get; set; }
    public bool IsActive { get; set; }
}
