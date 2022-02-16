using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using NextflowRunnerClient.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;


namespace NextflowRunnerClient.Pages
{
    public partial class ExecutePipeline
    {

        [Inject]
        NextflowAPI NfAPI { get; set; } 

    [Parameter]
    public int Id { get; set; } = 0;
    protected bool SUBMITTED { get; set; } = false;



    public async void ExecuteJob()
        {
            SUBMITTED = true;
            await NfAPI.ExecutePipelineAsync(Id,"hi");
           
        }
    }
}
