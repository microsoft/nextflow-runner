using NextflowRunnerAPI.Models;
using NextflowRunnerAPI.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPipelineService, PipelineService>();
builder.Services.AddScoped<ISshClient, SshClient>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/pipelines", async (IPipelineService pipelineService) =>
{
    return await pipelineService.GetPipelinesAsync();
}).WithName("GetPipelines");

app.MapGet("/pipelines/{id}", async (int pipelineId, IPipelineService pipelineService) =>
{
    return await pipelineService.GetPipelineAsync(pipelineId);
})
.WithName("GetPipeline");

app.MapPost("/pipelines", async (Pipeline pipeline, IPipelineService pipelineService) =>
{
    return await pipelineService.CreatePipelineAsync(pipeline);
})
.WithName("CreatePipeline");

app.MapPut("/pipelines/{id}", async (int pipelineId, IPipelineService pipelineService) =>
{
    return await pipelineService.UpdatePipelineAsync(pipelineId);
})
.WithDisplayName("UpdatePipeline");

app.MapPost("/pipelines/{id}", async (int pipelineId, IPipelineService pipelineService, ISshClient sshClient) =>
{
    var pipeline = await pipelineService.GetPipelineAsync(pipelineId);

    await sshClient.ExecutePipelineAsync(pipeline);
})
.WithName("ExecutePipeline");

app.MapDelete("/pipelines/{id}", async (int pipelineId, IPipelineService pipelineService) =>
{
    await pipelineService.DeletePipelineAsync(pipelineId);
})
.WithName("DeletePipeline");

app.Run();