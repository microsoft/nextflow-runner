using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NextflowRunner.Models;
using Microsoft.EntityFrameworkCore;

namespace NextflowRunner.WeblogTracer
{
    public class WeblogTracer
    {
        private readonly NextflowRunnerContext _context;
        public WeblogTracer(NextflowRunnerContext context)
        {
            _context = context;
        }

        [FunctionName("WeblogTracer")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            if (data == null)
                return new BadRequestResult();

            var eventType = data["event"] as string;

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
}
