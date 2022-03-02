using Microsoft.AspNetCore.Components;
using NextflowRunnerClient.Services;


namespace NextflowRunnerClient.Pages;

public partial class RunsList
{
    [Inject]
    protected NextflowAPI NfAPI { get; set; }

    protected List<PipelineRun> Runs { get; set; }


    protected override async Task OnInitializedAsync()
    {
        var a = await NfAPI.GetAllPipelineRunAsync();
        Runs = a.ToList<PipelineRun>();
    }
}
