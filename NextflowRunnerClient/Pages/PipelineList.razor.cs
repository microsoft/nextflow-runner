using Microsoft.AspNetCore.Components;
using NextflowRunnerClient.Services;

namespace NextflowRunnerClient.Pages;

public partial class PipelineList
{
    [Inject]
    protected NextflowAPI NfAPI { get; set; }

    protected List<Pipeline> Pipelines { get; set; } = new List<Pipeline>();
    protected string Errors { get; set; } = "";

    protected async override Task OnInitializedAsync()
    {
        try
        {
            var pipelineCollection = await NfAPI.GetPipelinesAsync();

            Pipelines = pipelineCollection.ToList();
        }
        catch (ApiException ae)
        {
            Errors = ae.Message;
        }


    }
}