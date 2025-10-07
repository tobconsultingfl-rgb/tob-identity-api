using System;

namespace TOB.Identity.Domain.Models;

public class TenantDto
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; }
    public string TenantWebsite { get; set; }
    public string TenantAddress1 { get; set; }
    public string TenantAddress2 { get; set; }
    public string TenantCity { get; set; }
    public State TenantState { get; set; }
    public string TenantZip { get; set; }
    public string TenantPhoneNumber { get; set; }
    public string TenantFax { get; set; }        
    public string ContactFirstName { get; set; }
    public string ContactLastName { get; set; }
    public string ContactPhoneNumber { get; set; }
    public string ContactEmail { get; set; }

    public DateTime? CreatedDateTime { get; set; }
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedDateTime { get; set; }
    public bool IsActive { get; set; }
}
