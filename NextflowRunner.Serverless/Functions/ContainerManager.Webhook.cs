using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_Webhook")]
    public static async Task<HttpResponseMessage> Webhook(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "/webhook")]
HttpRequestMessage req,
    [DurableClient] IDurableOrchestrationClient client)
    {
        var requestBody = await req.Content.ReadAsStringAsync();

        dynamic data = JsonConvert.DeserializeObject(requestBody);

        var runName = data?.RunName;

        await client.RaiseEventAsync(runName + "-orchestration", "ContainerManager_WebhookTrigger", runName);

        return req.CreateResponse(HttpStatusCode.NoContent);
    }
}
