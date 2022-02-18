using Microsoft.AspNetCore.Components;

namespace NextflowRunnerClient.Components;

public partial class Pipeline
{
    [Parameter]
    public Services.Pipeline Pline { get; set; } = null;
}