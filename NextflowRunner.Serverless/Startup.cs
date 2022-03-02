using Azure.Core;
using Azure.Identity;
using Microsoft.Azure;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.Management.ContainerInstance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Microsoft.Rest;
using NextflowRunner.Models;
using NextflowRunner.Serverless;
using System;

[assembly: FunctionsStartup(typeof(Startup))]
namespace NextflowRunner.Serverless;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        FunctionsHostBuilderContext context = builder.GetContext();

        builder.ConfigurationBuilder
            .SetBasePath(context.ApplicationRootPath)
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var containerConfig = new ContainerConfiguration
        {
            ClientId = Environment.GetEnvironmentVariable("ContainerConfiguration:ClientId"),
            ClientSecret = Environment.GetEnvironmentVariable("ContainerConfiguration:ClientSecret"),
            TenantId = Environment.GetEnvironmentVariable("ContainerConfiguration:TenantId"),
            SubscriptionId = Environment.GetEnvironmentVariable("ContainerConfiguration:SubscriptionId"),
            ResourceGroupName = Environment.GetEnvironmentVariable("ContainerConfiguration:ResourceGroupName"),
            StorageName = Environment.GetEnvironmentVariable("ContainerConfiguration:StorageName"),
            StorageKey = Environment.GetEnvironmentVariable("ContainerConfiguration:StorageKey"),
            BatchRegion = Environment.GetEnvironmentVariable("ContainerConfiguration:BatchRegion"),
            BatchAccountName = Environment.GetEnvironmentVariable("ContainerConfiguration:BatchAccountName"),
            BatchKey = Environment.GetEnvironmentVariable("ContainerConfiguration:BatchKey")
        };

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");

        builder.Services.AddSingleton(containerConfig);

        builder.Services.AddDbContext<NextflowRunnerContext>(options => options.UseSqlServer(connectionString));

        builder.Services.AddScoped<ContainerInstanceManagementClient>(options =>
        {
            var auth = ConfidentialClientApplicationBuilder.Create(containerConfig.ClientId).WithClientSecret(containerConfig.ClientSecret).Build();

            //var defaultCredential = new DefaultAzureCredential();
            var authResult = auth.AcquireTokenForClient(new[] { "https://management.azure.com/.default" }).ExecuteAsync().Result;
            var creds = new TokenCredentials(authResult.AccessToken);

            var client = new ContainerInstanceManagementClient(creds);
            client.SubscriptionId = containerConfig.SubscriptionId;
            return client;
        });
    }
}