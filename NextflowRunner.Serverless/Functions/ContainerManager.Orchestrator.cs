using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NextflowRunner.Models;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    private readonly ContainerConfiguration _containerConfig;
    private readonly Microsoft.Azure.Management.Fluent.IAzure _azure;
    private readonly NextflowRunnerContext _context;

    public ContainerManager(ContainerConfiguration containerConfig, NextflowRunnerContext context)
    {
        _context = context;
        _containerConfig = containerConfig;

        _azure = Microsoft.Azure.Management.Fluent.Azure
            .Configure()
            .Authenticate(SdkContext.AzureCredentialsFactory.FromServicePrincipal(
                _containerConfig.ClientId,
                _containerConfig.ClientSecret,
                _containerConfig.TenantId,
                AzureEnvironment.AzureGlobalCloud
            )).WithSubscription(_containerConfig.SubscriptionId);
    }

    [FunctionName("ContainerManager")]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var containerRunRequest = context.GetInput<ContainerRunRequest>();

        var containerGroupId = await context.CallActivityAsync<string>("ContainerManager_CreateContainer", containerRunRequest);

        await context.WaitForExternalEvent("WeblogTraceComplete");

        await context.CallActivityAsync("ContainerManager_DestroyContainer", containerGroupId);
    }
}
