#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TOB.Identity.Infrastructure.Data.Entities;

public partial class UserRoleMapping
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
    public Guid? CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? CreatedDateTime { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedDateTime { get; set; }

    [ForeignKey("RoleId")]
    [InverseProperty("UserRoleMappings")]
    public virtual Role Role { get; set; }
    [ForeignKey("UserId")]
    [InverseProperty("UserRoleMappings")]
    public virtual User User { get; set; }
}