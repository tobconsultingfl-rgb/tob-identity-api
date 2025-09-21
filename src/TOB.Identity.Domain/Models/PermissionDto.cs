using System;
using System.Collections.Generic;
using System.Linq;
namespace TOB.Identity.Domain.Models;

public class PermissionDto
{
    public Guid? PermissionId { get; set; }
    public string PermissionName { get; set; }
    public bool IsActive { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public DateTime? CreatedDateTime { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public int? SortOrder { get; set; }
}
