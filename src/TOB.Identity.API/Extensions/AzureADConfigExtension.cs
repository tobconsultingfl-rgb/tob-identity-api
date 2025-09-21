using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Azure.Identity;
using TOB.Identity.Domain.AppSettings;

namespace TOB.Identity.API.Extensions;

public static class AzureADConfigExtension
{
    public static IServiceCollection ConfigureGraphClient(this IServiceCollection services, IConfiguration configuration)
    {
        var graphConfig = configuration.GetSection(nameof(AzureAd));
        var clientSecretCredential = new ClientSecretCredential(
            graphConfig["TenantId"], graphConfig["ClientId"], graphConfig["ClientSecret"]);

        services.AddScoped(sp =>
        {
            return new GraphServiceClient(clientSecretCredential);
        });

        return services;
    }
}
