using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            ContainerImage = Environment.GetEnvironmentVariable("ContainerConfiguration:ContainerImage"),
        };

        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings:DefaultConnection");

        builder.Services.AddSingleton(containerConfig);

        builder.Services.AddDbContext<NextflowRunnerContext>(options => options.UseSqlServer(connectionString));
    }
}