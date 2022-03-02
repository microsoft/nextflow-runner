using Microsoft.Azure.Management.ContainerInstance;
using Microsoft.Azure.Management.ContainerInstance.Models;
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
    private readonly ContainerInstanceManagementClient _containerInstanceClient;
    private readonly List<EnvironmentVariable> _containerEnvVariables;

    public ContainerManager(ContainerInstanceManagementClient client, ContainerConfiguration containerConfig)
    {
        _containerInstanceClient = client;
        _containerConfig = containerConfig;

        _containerEnvVariables = new List<EnvironmentVariable>
        {
            new EnvironmentVariable{ Name = nameof(containerConfig.StorageName), Value = containerConfig.StorageName },
            new EnvironmentVariable{ Name = nameof(containerConfig.StorageKey),  Value = containerConfig.StorageKey },
            new EnvironmentVariable{ Name = nameof(containerConfig.BatchRegion), Value = containerConfig.BatchRegion },
            new EnvironmentVariable{ Name = nameof(containerConfig.BatchAccountName), Value = containerConfig.BatchAccountName },
            new EnvironmentVariable{ Name = nameof(containerConfig.BatchKey), Value = containerConfig.BatchKey }
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
