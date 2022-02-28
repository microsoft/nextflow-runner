using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace NextflowRunnerClient.Components
{
    public partial class RunListComponent
    {
       [Parameter]
       public List<Services.PipelineRun> RunList { get; set; }
    }
}
