using Microsoft.AspNetCore.Components;

namespace NextflowRunnerClient.Components;

public partial class RunListComponent
{
    [Parameter]
    public List<Services.PipelineRun> RunList { get; set; }
}
