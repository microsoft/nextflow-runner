using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NextflowRunner.Models;
using System.IO;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public class WeblogTracer
{
    private readonly NextflowRunnerContext _context;
    public WeblogTracer(NextflowRunnerContext context)
    {
        _context = context;
    }

    [FunctionName("WeblogTracer")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
    [DurableClient] IDurableOrchestrationClient client)
    {
        var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        dynamic data = JsonConvert.DeserializeObject(requestBody);

        if (data == null)
            return new BadRequestResult();

        var properties = data as JObject;

        var eventType = properties.SelectToken("event").Value<string>();

        if (!eventType.StartsWith("process_"))
            return new BadRequestResult();

        var runName = data?.runName as string;

        var pipeline = await _context.PipelineRuns.FirstOrDefaultAsync(r => string.Equals(r.PipelineRunName, runName));

        if (pipeline == null)
            return new NotFoundResult();

        pipeline.Status = eventType;

        await _context.SaveChangesAsync();

        // todo: double check the eventType
        if(eventType == "process_completed")
            await client.RaiseEventAsync(runName + "-orchestration", "ContainerManager_WebhookTrigger", runName);
        // use the run name as the orchestrationId to reduce information needed to pass

        return new NoContentResult();
    }
}
