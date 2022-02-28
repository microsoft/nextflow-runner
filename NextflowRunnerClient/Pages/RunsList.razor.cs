using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.VisualBasic;
using NextflowRunnerClient.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NextflowRunnerClient.Pages
{
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
}
            