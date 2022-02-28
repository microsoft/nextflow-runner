using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NextflowRunner.API;
using NextflowRunner.API.Models;
using Renci.SshNet;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<SSHConnectionOptions>(builder.Configuration.GetSection(SSHConnectionOptions.ConfigSection));
builder.Services.Configure<AzureContainerOptions>(builder.Configuration.GetSection(AzureContainerOptions.ConfigSection));

builder.Services.AddDbContext<NextflowRunnerContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(o =>
    o.AddDefaultPolicy(b =>
        b.AllowAnyHeader()
        .AllowAnyOrigin()
        .AllowAnyMethod()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseHttpsRedirection();

#region Pipelines

app.MapGet("/pipelines", async (NextflowRunnerContext db) =>
{
    return await db.Pipelines
        .Include(p => p.PipelineParams)
        .Include(p => p.PipelineRuns)
        .ToListAsync();
})
.Produces<List<Pipeline>>(StatusCodes.Status200OK)
.WithName("GetPipelines");

app.MapGet("/pipelines/{pipelineId}", async (int pipelineId, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines
        .Include(p => p.PipelineParams)
        .Include(p => p.PipelineRuns)
        .FirstOrDefaultAsync(p => p.PipelineId == pipelineId);

    if (pipeline == null) return Results.NotFound();

    return Results.Ok(pipeline);
})
.Produces<Pipeline>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetPipeline");

app.MapPost("/pipelines", async ([FromBody] Pipeline pipeline, NextflowRunnerContext db) =>
{
    pipeline.PipelineParams ??= new List<PipelineParam>();
    pipeline.PipelineRuns = new List<PipelineRun>();

    db.Pipelines.Add(pipeline);

    await db.SaveChangesAsync();

    return Results.CreatedAtRoute("GetPipeline", new { pipelineId = pipeline.PipelineId }, pipeline);
})
.Produces<Pipeline>(StatusCodes.Status201Created)
.WithName("CreatePipeline");

app.MapPut("/pipelines/{pipelineId}", async (int pipelineId, [FromBody] Pipeline pipeline, NextflowRunnerContext db) =>
{
    var dbPipeline = await db.Pipelines.FindAsync(pipeline.PipelineId);

    if (dbPipeline == null) return Results.NotFound();

    dbPipeline.PipelineName = pipeline.PipelineName;
    dbPipeline.Description = pipeline.Description;
    dbPipeline.GitHubUrl = pipeline.GitHubUrl;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithName("UpdatePipeline");

app.MapPost("/pipelines/{pipelineId}", async (int pipelineId, ExecutionRequest execReq, NextflowRunnerContext db, IOptions<SSHConnectionOptions> sshConnectionOptions) =>
{
    var wut = sshConnectionOptions.Value.WEBLOG_URL;

    var pipeline = await db.Pipelines.Include(p => p.PipelineParams).FirstOrDefaultAsync(p => p.PipelineId == pipelineId);

    if (pipeline == null) return Results.NotFound();

    var commandStr = "/home/azureuser/tools/nextflow run";

    var filename = $" {pipeline.GitHubUrl}";

    commandStr += $"{filename} -name {execReq.RunName} -bg -with-weblog \"{sshConnectionOptions.Value.WEBLOG_URL}\"";

    if (pipeline.PipelineParams != null)
        foreach (var param in pipeline.PipelineParams)
        {
            var value = execReq.Parameters[param.PipelineParamId] ?? param.DefaultValue;

            var sanitizedValue = value.ReplaceLineEndings();

            commandStr += $" --{param.ParamName} {sanitizedValue}";
        }

    var run = new PipelineRun
    {
        PipelineId = pipelineId,
        PipelineRunName = execReq.RunName,
        NextflowRunCommand = commandStr,
        RunDateTime = DateTime.UtcNow
    };

    pipeline.PipelineRuns ??= new List<PipelineRun>();

    pipeline.PipelineRuns.Add(run);

    await db.SaveChangesAsync();

    using var client = new SshClient(
        sshConnectionOptions.Value.VM_ADMIN_HOSTNAME,
        sshConnectionOptions.Value.VM_ADMIN_USERNAME,
        sshConnectionOptions.Value.VM_ADMIN_PASSWORD);

    client.Connect();

    using var command = client.CreateCommand(run.NextflowRunCommand);

    var output = command.Execute();

    client.Disconnect();

    return Results.CreatedAtRoute("GetPipelineRun", new { pipelineId = pipeline.PipelineId, pipelineRunId = run.PipelineRunId }, run);
})
.Produces<PipelineRun>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status404NotFound)
.WithName("ExecutePipeline");

app.MapDelete("/pipelines/{pipelineId}", async (int pipelineId, NextflowRunnerContext db) =>
{
    var dbPipeline = await db.Pipelines.FindAsync(pipelineId);

    if (dbPipeline == null) return Results.NotFound();

    db.Pipelines.Remove(dbPipeline);

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithName("DeletePipeline");

#endregion

#region PipelineParams

app.MapGet("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines
    .Include(p => p.PipelineParams)
    .FirstOrDefaultAsync(p => p.PipelineId == pipelineId);

    if (pipeline == null) return Results.NotFound();

    return Results.Ok(pipeline.PipelineParams);
})
.Produces<List<PipelineParam>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetPipelineParams");

app.MapGet("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, NextflowRunnerContext db) =>
{
    var pipelineParam = await db.PipelineParams.FindAsync(pipelineParamId);

    if (pipelineParam == null) return Results.NotFound();

    return Results.Ok(pipelineParam);
})
.Produces<PipelineParam>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetPipelineParam");

app.MapPost("/pipelines/{pipelineId}/pipelineparams", async (int pipelineId, [FromBody] PipelineParam pipelineParam, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines
        .Include(p => p.PipelineParams)
        .FirstOrDefaultAsync(p => p.PipelineId == pipelineId);

    if (pipeline == null) return Results.NotFound();

    pipeline.PipelineParams ??= new List<PipelineParam>();

    pipeline.PipelineParams.Add(pipelineParam);

    await db.SaveChangesAsync();

    return Results.CreatedAtRoute("GetPipelineParam", new { pipelineId, pipelineParamId = pipelineParam.PipelineParamId }, pipelineParam);
})
.Produces<PipelineParam>(StatusCodes.Status201Created)
.Produces(StatusCodes.Status404NotFound)
.WithName("CreatePipelineParam");

app.MapPut("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, [FromBody] PipelineParam pipelineParam, NextflowRunnerContext db) =>
{
    var dbPipelineParam = await db.PipelineParams.FindAsync(pipelineParam.PipelineParamId);

    if (dbPipelineParam == null) return Results.NotFound();

    dbPipelineParam.ParamName = pipelineParam.ParamName;
    dbPipelineParam.ParamType = pipelineParam.ParamType;
    dbPipelineParam.DefaultValue = pipelineParam.DefaultValue;
    dbPipelineParam.ParamExample = pipelineParam.ParamExample;

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithName("UpdatePipelineParam");

app.MapDelete("/pipelines/{pipelineId}/pipelineparams/{pipelineParamId}", async (int pipelineId, int pipelineParamId, NextflowRunnerContext db) =>
{
    var dbPipelineParam = await db.PipelineParams.FindAsync(pipelineParamId);

    if (dbPipelineParam == null) return Results.NotFound();

    db.PipelineParams.Remove(dbPipelineParam);

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithName("DeletePipelineParam");

#endregion

#region PipelineRuns

app.MapGet("/pipelines/{pipelineId}/pipelineruns", async (int pipelineId, NextflowRunnerContext db) =>
{
    var pipeline = await db.Pipelines
    .Include(p => p.PipelineRuns)
    .FirstOrDefaultAsync(p => p.PipelineId == pipelineId);

    if (pipeline == null) return Results.NotFound();

    return Results.Ok(pipeline.PipelineRuns);
})
.Produces<List<PipelineRun>>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetPipelineRuns");

app.MapGet("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, NextflowRunnerContext db) =>
{
    var pipelineRun = await db.PipelineRuns.FindAsync(pipelineRunId);

    if (pipelineRun == null) return Results.NotFound();

    return Results.Ok(pipelineRun);
})
.Produces<PipelineRun>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetPipelineRun");

app.MapDelete("/pipelines/{pipelineId}/pipelineruns/{pipelineRunId}", async (int pipelineId, int pipelineRunId, NextflowRunnerContext db) =>
{
    var dbPipelineRun = await db.PipelineRuns.FindAsync(pipelineRunId);

    if (dbPipelineRun == null) return Results.NotFound();

    db.PipelineRuns.Remove(dbPipelineRun);

    await db.SaveChangesAsync();

    return Results.NoContent();
})
.Produces(StatusCodes.Status204NoContent)
.Produces(StatusCodes.Status404NotFound)
.WithName("DeletePipelineRun");

#endregion

#region AzureStorage

app.MapGet("/azsas/{authKey}", (string authKey, IOptions<AzureContainerOptions> azureContainerOptions) =>
{
    if (authKey == azureContainerOptions.Value.AZURE_STORAGE_KEY) return Results.Ok(azureContainerOptions.Value);

    return Results.NotFound();
})
.Produces<AzureContainerOptions>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound)
.WithName("GetAzureContainerOptions");


#endregion



app.Run();