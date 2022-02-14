using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextflowRunnerAPI;
using NextflowRunnerAPI.Models;
using Renci.SshNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SSHConnectionOptions>(builder.Configuration.GetSection(SSHConnectionOptions.ConfigSection));

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
    var pipeline = await db.Pipelines.FindAsync(pipelineId);

    if (pipeline == null) return Results.NotFound();

    return Results.Ok(pipeline);
})
.WithName("GetPipeline");

app.MapPost("/pipelines", async ([FromBody] Pipeline pipeline, NextflowRunnerContext db) =>
{
    db.Pipelines.Add(pipeline);

    await db.SaveChangesAsync();

    return Results.CreatedAtRoute("GetPipeline", new { pipelineId = pipeline.PipelineId }, pipeline);
})
.WithName("CreatePipeline");

app.MapPut("/pipelines/{pipelineId}", async (int pipelineId, [FromBody] Pipeline pipeline, NextflowRunnerContext db) =>
{
    var dbPipeline = await db.Pipelines.FindAsync(pipeline.PipelineId);

    if (dbPipeline == null) return null;

    dbPipeline = pipeline;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("UpdatePipeline");

app.MapPost("/pipelines/{pipelineId}", async (int pipelineId, IDictionary<int, string> formParams, NextflowRunnerContext db, IOptions<SSHConnectionOptions> sshConnectionOptions) =>
{
    //var pipeline = await db.Pipelines.FindAsync(pipelineId);

    //var commandStr = "nextflow";

    //// for mvp just use hardcoded pipeline file; potentially get from AZ Storage later?
    //// for now, we're repurposing github url string as the folder until we can automate fetching of the files
    //var filename = $" {pipeline.GitHubUrl}";

    //commandStr += filename;

    //if (pipeline.PipelineParams != null)
    //    foreach (var param in pipeline.PipelineParams)
    //    {
    //        var value = formParams[param.PipelineParamId] ?? param.DefaultValue;

    //        // todo: santize

    //        commandStr += $" --{param.ParamName} {value}";
    //    }

    //var run = new PipelineRun
    //{
    //    PipelineId = pipelineId,
    //    NextflowRunCommand = commandStr,
    //    RunDateTime = DateTime.UtcNow, // now vs utc now?
    //    Status = "running" // what are statuses can nextflow have? do we need to extend with any of our own? define in a type table or an enum?
    //};

    //// running, failed, succeeded

    //pipeline.PipelineRuns ??= new List<PipelineRun>();

    //pipeline.PipelineRuns.Add(run);

    //await db.SaveChangesAsync();

    using var client = new SshClient(
        sshConnectionOptions.Value.VM_ADMIN_HOSTNAME,
        sshConnectionOptions.Value.VM_ADMIN_USERNAME,
        sshConnectionOptions.Value.VM_ADMIN_PASSWORD);

    client.Connect();

    using var command = client.CreateCommand("/bin/nextflow -h");
    //using var command = client.CreateCommand(run.commandStr);

    // update run entry with status

    var output = command.Execute();

    client.Disconnect();

    // todo: output to azure storage

    return output;
})
.WithName("ExecutePipeline");

app.MapDelete("/pipelines/{pipelineId}", async (int pipelineId, NextflowRunnerContext db) =>
{
    var dbPipeline = await db.Pipelines.FindAsync(pipelineId);

    if (dbPipeline == null) return Results.NotFound();

    db.Pipelines.Remove(dbPipeline);

    await db.SaveChangesAsync();

    return Results.NoContent();
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
    var pipelineParam = await db.PipelineParams.FindAsync(pipelineParamId);

    if (pipelineParam == null) return Results.NotFound();

    return Results.Ok(pipelineParam);
})
.WithName("GetPipelineParam");

app.MapPost("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, [FromBody] PipelineParam pipelineParam, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines.FindAsync(pipelineId);

    if (pipeline == null) return Results.NotFound();

    pipeline.PipelineParams ??= new List<PipelineParam>();

    pipeline.PipelineParams.Add(pipelineParam);

    await db.SaveChangesAsync();

    return Results.CreatedAtRoute("GetPipelineParam", new { pipelineId = pipelineId, pipelineParamId = pipelineParam.PipelineParamId }, pipelineParam);
})
.WithName("CreatePipelineParam");

app.MapPut("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, [FromBody] PipelineParam pipelineParam, NextflowRunnerContext db) =>
{
    var dbPipelineParam = await db.PipelineParams.FindAsync(pipelineParam.PipelineParamId);

    if (dbPipelineParam == null) return Results.NotFound();

    dbPipelineParam = pipelineParam;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("UpdatePipelineParam");

app.MapDelete("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, NextflowRunnerContext db) =>
{
    var dbPipelineParam = await db.PipelineParams.FindAsync(pipelineParamId);

    if (dbPipelineParam == null) return Results.NotFound();

    db.PipelineParams.Remove(dbPipelineParam);

    await db.SaveChangesAsync();

    return Results.NoContent();
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
    var pipelineRun = await db.PipelineRuns.FindAsync(pipelineRunId);

    if (pipelineRun == null) return Results.NotFound();

    return Results.Ok(pipelineRun);
})
.WithName("GetPipelineRun");

app.MapPost("/pipelines/{pipelineId}/pipelineruns", async (int pipelineId, [FromBody] PipelineRun pipelineRun, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines.FindAsync(pipelineId);

    if (pipeline == null) return Results.NotFound();

    pipeline.PipelineRuns ??= new List<PipelineRun>();

    pipeline.PipelineRuns.Add(pipelineRun);

    await db.SaveChangesAsync();

    return Results.CreatedAtRoute("GetPipelineRun", new { pipelineId = pipelineId, pipelineRunId = pipelineRun.PipelineRunId }, pipelineRun);
})
.WithName("CreatePipelineRun");

app.MapPut("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, [FromBody] PipelineRun pipelineRun, NextflowRunnerContext db) =>
{
    var dbPipelineRun = await db.PipelineRuns.FindAsync(pipelineRun.PipelineRunId);

    if (dbPipelineRun == null) return Results.NotFound();

    dbPipelineRun = pipelineRun;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("UpdatePipelineRun");

app.MapDelete("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, NextflowRunnerContext db) =>
{
    var dbPipelineRun = await db.PipelineRuns.FindAsync(pipelineRunId);

    if (dbPipelineRun == null) return Results.NotFound();

    db.PipelineRuns.Remove(dbPipelineRun);

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.WithName("DeletePipelineRun");

#endregion

app.Run();