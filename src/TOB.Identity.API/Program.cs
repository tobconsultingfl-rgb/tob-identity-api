using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;
using System;


namespace TOB.Identity.API;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                var builtConfig = config.Build();
                var keyVaultEndpoint = builtConfig["KeyVault:VaultUri"];

                if (!string.IsNullOrEmpty(keyVaultEndpoint))
                {
                    config.AddAzureKeyVault(
                        new Uri(keyVaultEndpoint),
                        new DefaultAzureCredential());
                }
            })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseIISIntegration().UseStartup<Startup>();
            });
}
