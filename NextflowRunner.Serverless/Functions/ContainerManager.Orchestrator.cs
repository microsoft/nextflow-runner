using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NextflowRunner.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using TimeSpan = System.TimeSpan;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    private readonly ContainerConfiguration _containerConfig;
    private readonly Microsoft.Azure.Management.Fluent.IAzure _azure;
    private readonly Dictionary<string, string> _containerEnvVariables;

    public ContainerManager(ContainerConfiguration containerConfig)
    {
        _containerConfig = containerConfig;

        _azure = Microsoft.Azure.Management.Fluent.Azure
            .Configure()
            .Authenticate(SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                _containerConfig.ClientId,
                _containerConfig.ClientSecret,
                _containerConfig.TenantId,
                AzureEnvironment.AzureGlobalCloud
            )).WithSubscription(_containerConfig.SubscriptionId);

        _containerEnvVariables = new Dictionary<string, string>
        {
            [nameof(containerConfig.StorageName)] = containerConfig.StorageName,
            [nameof(containerConfig.StorageKey)] = containerConfig.StorageKey,
            [nameof(containerConfig.BatchRegion)] = containerConfig.BatchRegion,
            [nameof(containerConfig.BatchAccountName)] = containerConfig.BatchAccountName,
            [nameof(containerConfig.BatchKey)] = containerConfig.BatchKey,
        };
    }

    [FunctionName("ContainerManager")]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var containerRunRequest = context.GetInput<ContainerRunRequest>();

        var containerGroupId = string.Empty;

        if (!context.IsReplaying)
            containerGroupId = await context.CallActivityAsync<string>("ContainerManager_CreateContainer", containerRunRequest);

        await context.WaitForExternalEvent("WeblogTraceComplete", TimeSpan.FromHours(48));

        await context.CallActivityAsync("ContainerManager_DestroyContainer", containerGroupId);
    }
}
