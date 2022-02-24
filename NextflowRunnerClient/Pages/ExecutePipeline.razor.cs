using Microsoft.AspNetCore.Components;
using NextflowRunnerClient.Models;
using NextflowRunnerClient.Services;

namespace NextflowRunnerClient.Pages;

public partial class ExecutePipeline
{
    [Inject]
    NextflowAPI NfAPI { get; set; }

    [Parameter]
    public int Id { get; set; } = 0;

    protected string RunNameUqEnd { get; set; } = Guid.NewGuid().ToString()[..6];
    protected Pipeline Pipeline { get; set; } = new ();
    protected ICollection<ViewParam> Params { get; set; } = new List<ViewParam>();
    protected ExecutionRequest ExecutionRequest { get; set; } = new() { RunName = string.Empty };
    protected bool SUBMITTED { get; set; } = false;
    protected bool VALID { get; set; } = false;

    protected async override Task OnInitializedAsync()
    {
        Pipeline = await NfAPI.GetPipelineAsync(Id);

        Params = Pipeline.PipelineParams.Select(p => new ViewParam(p)).ToList();
    }

    public async void ExecuteJob()
    {
        SUBMITTED = true;

        ExecutionRequest.RunName += RunNameUqEnd;
        ExecutionRequest.Parameters = Params
            .ToDictionary(p => p.PipelineParamId.ToString(), p => p.Value);

        await NfAPI.ExecutePipelineAsync(Id, ExecutionRequest);
    }
}