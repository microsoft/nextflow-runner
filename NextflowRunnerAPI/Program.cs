using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NextflowRunnerAPI;
using NextflowRunnerAPI.Models;
using Renci.SshNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<NextflowRunnerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

#region Pipelines

app.MapGet("/pipelines", async (NextflowRunnerContext db) =>
{
    return await db.Pipelines.ToListAsync();
}).WithName("GetPipelines");

app.MapGet("/pipelines/{pipelineId}", async (int pipelineId, NextflowRunnerContext db) =>
{
    return await db.Pipelines.FindAsync(pipelineId);
})
.WithName("GetPipeline");

app.MapPost("/pipelines", async ([FromBody] Pipeline pipeline, NextflowRunnerContext db) =>
{
    db.Pipelines.Add(pipeline);
    await db.SaveChangesAsync();
    return pipeline;
})
.WithName("CreatePipeline");

app.MapPut("/pipelines/{pipelineId}", async (int pipelineId, [FromBody] Pipeline pipeline, NextflowRunnerContext db) =>
{
    var dbPipeline = await db.Pipelines.FindAsync(pipeline.PipelineId);

    if (dbPipeline == null) return null;

    dbPipeline = pipeline;

    await db.SaveChangesAsync();

    return pipeline;
})
.WithDisplayName("UpdatePipeline");

app.MapPost("/pipelines/{pipelineId}", async (int pipelineId, string runCommand, NextflowRunnerContext db, IConfiguration config) =>
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

app.MapDelete("/pipelines/{pipelineId}", async (int pipelineId, NextflowRunnerContext db) =>
{
    var dbPipeline = await db.Pipelines.FindAsync(pipelineId);

    if (dbPipeline == null) return;

    db.Pipelines.Remove(dbPipeline);

    await db.SaveChangesAsync();
})
.WithName("DeletePipeline");

#endregion

#region PipelineParams

app.MapGet("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, NextflowRunnerContext db) =>
{
    return await db.PipelineParams.Where(p => p.PipelineId == pipelineId).ToListAsync();
}).WithName("GetPipelineParams");

app.MapGet("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, NextflowRunnerContext db) =>
{
    return await db.PipelineParams.FindAsync(pipelineParamId);
})
.WithName("GetPipelineParam");

app.MapPost("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, [FromBody] PipelineParam pipelineParam, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines.FindAsync(pipelineId);

    if (pipeline == null) return null;

    pipeline.PipelineParams ??= new List<PipelineParam>();

    pipeline.PipelineParams.Add(pipelineParam);

    await db.SaveChangesAsync();

    return pipelineParam;
})
.WithName("CreatePipelineParam");

app.MapPut("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, [FromBody] PipelineParam pipelineParam, NextflowRunnerContext db) =>
{
    var dbPipelineParam = await db.PipelineParams.FindAsync(pipelineParam.PipelineParamId);

    if (dbPipelineParam == null) return null;

    dbPipelineParam = pipelineParam;

    await db.SaveChangesAsync();

    return pipelineParam;
})
.WithDisplayName("UpdatePipelineParam");

app.MapDelete("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, NextflowRunnerContext db) =>
{
    var dbPipelineParam = await db.PipelineParams.FindAsync(pipelineParamId);

    if (dbPipelineParam == null) return;

    db.PipelineParams.Remove(dbPipelineParam);

    await db.SaveChangesAsync();
})
.WithName("DeletePipelineParam");

#endregion

#region PipelineRuns

app.MapGet("/pipelines/{pipelineId}/pipelineruns", async (int pipelineId, NextflowRunnerContext db) =>
{
    return await db.PipelineRuns.Where(p => p.PipelineId == pipelineId).ToListAsync();
}).WithName("GetPipelineRuns");

app.MapGet("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, NextflowRunnerContext db) =>
{
    return await db.PipelineRuns.FindAsync(pipelineRunId);
})
.WithName("GetPipelineRun");

app.MapPost("/pipelines/{pipelineId}/pipelineruns", async (int pipelineId, [FromBody] PipelineRun pipelineRun, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines.FindAsync(pipelineId);

    if (pipeline == null) return null;

    pipeline.PipelineRuns ??= new List<PipelineRun>();

    pipeline.PipelineRuns.Add(pipelineRun);

    await db.SaveChangesAsync();

    return pipelineRun;
})
.WithName("CreatePipelineRun");

app.MapPut("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, [FromBody] PipelineRun pipelineRun, NextflowRunnerContext db) =>
{
    var dbPipelineRun = await db.PipelineRuns.FindAsync(pipelineRun.PipelineRunId);

    if (dbPipelineRun == null) return null;

    dbPipelineRun = pipelineRun;

    await db.SaveChangesAsync();

    return pipelineRun;
})
.WithDisplayName("UpdatePipelineRun");

app.MapDelete("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, NextflowRunnerContext db) =>
{
    var dbPipelineRun = await db.PipelineRuns.FindAsync(pipelineRunId);

    if (dbPipelineRun == null) return;

    db.PipelineRuns.Remove(dbPipelineRun);

    await db.SaveChangesAsync();
})
.WithName("DeletePipelineRun");

#endregion

app.Run();