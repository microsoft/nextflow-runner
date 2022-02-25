using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.AppService.Fluent.Models;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextflowRunner.Models;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    private readonly ContainerConfiguration _containerConfig;

    public ContainerManager(ContainerConfiguration containerConfig)
    {
        _containerConfig = containerConfig;
    }

    [FunctionName("ContainerManager")]
    public async Task RunOrchestrator(
        [OrchestrationTrigger] IDurableOrchestrationContext context)
    {
        var execReq = context.GetInput<ExecutionRequest>();

        var output1 = await context.CallActivityAsync<object>("ContainerManager_CreateContainer", execReq);

        var output2 = await context.WaitForExternalEvent<object>("ContainerManager_WebhookTrigger");

        await context.CallActivityAsync("ContainerManager_DestroyContainer", output2);
    }
}
