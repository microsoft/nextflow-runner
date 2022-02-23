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

    protected ICollection<ViewParam> Params { get; set; } = new List<ViewParam>();
    protected ExecutionRequest ExecutionRequest { get; set; } = new() { RunName = Guid.NewGuid().ToString().Substring(0,6) };
    protected bool SUBMITTED { get; set; } = false;
    protected bool VALID { get; set; } = false;

    protected async override Task OnInitializedAsync()
    {
        var apiParams = await NfAPI.GetPipelineParamsAsync(Id);

        Params = apiParams.Select(p => new ViewParam(p)).ToList();
    }

    public async void ExecuteJob()
    {
        SUBMITTED = true;

        ExecutionRequest.Parameters = Params
            .ToDictionary(p => p.PipelineParamId.ToString(), p => p.Value);

        // todo: revisit for updated api spec
        try
        {
            await NfAPI.ExecutePipelineAsync(Id, ExecutionRequest);
        } catch (Exception ex)
        {
            throw ex;
        }
    }
}