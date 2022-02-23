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

        var dictionary = Params.ToDictionary(p => p.PipelineParamId.ToString(), p => p.Value);

        var result = string.Join(',', dictionary.Select(kvp => kvp.Value).ToArray());

        // todo: revisit for updated api spec
        await NfAPI.ExecutePipelineAsync(Id, dictionary);
    }
}