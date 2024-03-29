﻿using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using NextflowRunner.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_HttpStart")]
    public static async Task<HttpResponseMessage> HttpStart(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestMessage req,
        [DurableClient] IDurableOrchestrationClient starter)
    {
        var json = await req.Content.ReadAsStringAsync();

        var containerRunRequest = JsonConvert.DeserializeObject<ContainerRunRequest>(json);

        // use the run name as the orchestrationId to reduce information needed to pass
        var instanceId = await starter.StartNewAsync("ContainerManager", containerRunRequest.RunName + "-orchestration", containerRunRequest);

        return starter.CreateCheckStatusResponse(req, instanceId);
    }
}
