using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using NextflowRunnerClient.Components;
using NextflowRunnerClient.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Pipeline = NextflowRunnerClient.Services.Pipeline;

namespace NextflowRunnerClient.Pages
{
    public partial class ExecutePipeline
    {

        [Inject]
        NextflowAPI NfAPI { get; set; } 

    [Parameter]
    public int Id { get; set; } = 0;

        protected ICollection<Services.PipelineParam> Params { get; set; } = null;
        protected Dictionary<string, string> ParamValue { get; set; } = new Dictionary<string, string>();
        protected bool SUBMITTED { get; set; } = false;
        protected bool VALID { get; set; } = false;

        protected async override Task OnInitializedAsync()
        {
            Params = await NfAPI.GetPipelineParamsAsync(Id);
            ParamValue=Params.ToDictionary(p => p.PipelineParamId.ToString(), p => "");
            
        }

        public async void ExecuteJob()
        {
            SUBMITTED = true;
            
            await NfAPI.ExecutePipelineAsync(Id,ParamValue);
           
        }
        public void ValidateParams()
        {
            VALID = true;
            foreach(var pvalue in ParamValue)
            {
                if (string.IsNullOrEmpty(pvalue.Value))
                {
                    VALID = false;
                    return;
                }
            }
            
        }
    }
}
