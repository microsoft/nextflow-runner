using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextflowRunnerAPI;
using NextflowRunnerAPI.Models;
using NextflowRunnerAPI.Services;
using Renci.SshNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPipelineService, PipelineService>();
builder.Services.AddScoped<IPipelineParamService, PipelineParamService>();
builder.Services.AddScoped<IPipelineRunService, PipelineRunService>();

builder.Services.AddDbContext<NextflowRunnerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

#region Pipelines

app.MapGet("/pipelines", async (IPipelineService pipelineService) =>
{
    return await pipelineService.GetPipelinesAsync();
}).WithName("GetPipelines");

app.MapGet("/pipelines/{pipelineId}", async (int pipelineId, IPipelineService pipelineService) =>
{
    return await pipelineService.GetPipelineAsync(pipelineId);
})
.WithName("GetPipeline");

app.MapPost("/pipelines", async ([FromBody] Pipeline pipeline, IPipelineService pipelineService) =>
{
    return await pipelineService.CreatePipelineAsync(pipeline);
})
.WithName("CreatePipeline");

app.MapPut("/pipelines/{pipelineId}", async (int pipelineId, [FromBody] Pipeline pipeline, IPipelineService pipelineService) =>
{
    return await pipelineService.UpdatePipelineAsync(pipeline);
})
.WithDisplayName("UpdatePipeline");

app.MapPost("/pipelines/{pipelineId}", async (int pipelineId, string runCommand, IPipelineService pipelineService, IConfiguration config) =>
{
    await Task.Delay(0);

    throw new NotImplementedException();
    //var pipeline = await pipelineService.GetPipelineAsync(pipelineId);

    //var run = new PipelineRun 
    //{
    //    PipelineId = pipelineId,
    //    NextflowRunCommand = runCommand ?? "./nextflow ./hello/main.nf",
    //    RunDateTime = DateTime.UtcNow, // now vs utc now?
    //    Status = "running" // what are statuses can nextflow have? do we need to extend with any of our own? define in a type table or an enum?
    //};

    //pipeline.Runs.Add(run);

    //using var client = new SshClient(config["SSHConnection:VM_ADMIN_HOSTNAME"], config["SSHConnection:VM_ADMIN_USERNAME"], config["SSHConnection:VM_ADMIN_PASSWORD"]);

    //using var command = client.CreateCommand(run.NextflowRunCommand);
})
.WithName("ExecutePipeline");

app.MapDelete("/pipelines/{pipelineId}", async (int pipelineId, IPipelineService pipelineService) =>
{
    await pipelineService.DeletePipelineAsync(pipelineId);
})
.WithName("DeletePipeline");

#endregion

#region PipelineParams

app.MapGet("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, IPipelineParamService pipelineParamService) =>
{
    return await pipelineParamService.GetPipelineParamsAsync(pipelineId);
}).WithName("GetPipelineParams");

app.MapGet("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, IPipelineParamService pipelineParamService) =>
{
    return await pipelineParamService.GetPipelineParamAsync(pipelineParamId);
})
.WithName("GetPipelineParam");

app.MapPost("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, [FromBody] PipelineParam pipelineParam, IPipelineParamService pipelineParamService) =>
{
    return await pipelineParamService.CreatePipelineParamAsync(pipelineId, pipelineParam);
})
.WithName("CreatePipelineParam");

app.MapPut("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, [FromBody] PipelineParam pipelineParam, IPipelineParamService pipelineParamService) =>
{
    return await pipelineParamService.UpdatePipelineParamAsync(pipelineParam);
})
.WithDisplayName("UpdatePipelineParam");

app.MapDelete("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, IPipelineParamService pipelineParamService) =>
{
    await pipelineParamService.DeletePipelineParamAsync(pipelineParamId);
})
.WithName("DeletePipelineParam");

#endregion

#region PipelineRuns

app.MapGet("/pipelines/{pipelineId}/pipelineruns", async (int pipelineId, IPipelineRunService pipelineRunService) =>
{
    return await pipelineRunService.GetPipelineRunsAsync(pipelineId);
}).WithName("GetPipelineRuns");

app.MapGet("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, IPipelineRunService pipelineRunService) =>
{
    return await pipelineRunService.GetPipelineRunAsync(pipelineRunId);
})
.WithName("GetPipelineRun");

app.MapPost("/pipelines/{pipelineId}/pipelineruns", async (int pipelineId, [FromBody] PipelineRun pipelineRun, IPipelineRunService pipelineRunService) =>
{
    return await pipelineRunService.CreatePipelineRunAsync(pipelineId, pipelineRun);
})
.WithName("CreatePipelineRun");

app.MapPut("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, [FromBody] PipelineRun pipelineRun, IPipelineRunService pipelineRunService) =>
{
    return await pipelineRunService.UpdatePipelineRunAsync(pipelineRun);
})
.WithDisplayName("UpdatePipelineRun");

app.MapDelete("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, IPipelineRunService pipelineRunService) =>
{
    await pipelineRunService.DeletePipelineRunAsync(pipelineRunId);
})
.WithName("DeletePipelineRun");

#endregion

app.Run();