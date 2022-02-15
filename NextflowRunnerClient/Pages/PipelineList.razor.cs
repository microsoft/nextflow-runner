using Microsoft.AspNetCore.Components;
using NextflowRunnerClient.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NextflowRunnerClient.Pages
{
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
                Pipelines = (List<Pipeline>)await NfAPI.GetPipelinesAsync();
            }catch (ApiException ae)
            {
                Errors = ae.Message;
            }
            
          
        }
    }
}
