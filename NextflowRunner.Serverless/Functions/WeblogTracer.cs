using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NextflowRunner.Models;
using System;
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
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
    {

        // 1) receive request (type execute)
        // 2) instantiate container
        // 3) sleep
        // 4) weblog tracer collects traces
        // 5) weblog tracer wakes sleeping process to kill container

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

        return new NoContentResult();
    }
}
