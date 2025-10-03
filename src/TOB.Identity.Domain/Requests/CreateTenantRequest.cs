using System.ComponentModel.DataAnnotations;

namespace TOB.Identity.Domain.Requests;

public class CreateTenantRequest
{
    [Required]
    public string TenantName { get; set; }
    [Required]
    public string TenantAddress1 { get; set; }
    public string TenantAddress2 { get; set; }
    [Required]
    public string TenantCity { get; set; }
    [Required]
    public State TenantState { get; set; }
    [Required]
    public string TenantZip { get; set; }
    [Required]
    public string TenantPhoneNumber { get; set; }
    public string TenantFax { get; set; }
    [Required]
    public string ContactFirstName { get; set; }
    [Required]
    public string ContactLastName { get; set; }
    [Required]
    public string ContactMobilePhone { get; set; }

    [Required]
    public string ContactEmail { get; set; }
    [Required]
    public string Password { get; set; }
}
