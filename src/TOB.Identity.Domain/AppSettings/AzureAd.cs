namespace TOB.Identity.Domain.AppSettings;

public class AzureAd
{
    public string Instance { get; set; }
    public string ClientId { get; set; }
    public string Domain { get; set; }
    public string SignedOutCallbackPath { get; set; }
    public string SignUpSignInPolicyId { get; set; }
    public string TenantId { get; set; }
    public string ClientSecret { get; set; }
    public string ExtensionId { get; set; }
}
