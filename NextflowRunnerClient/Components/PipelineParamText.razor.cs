using Microsoft.AspNetCore.Components;
using NextflowRunnerClient.Models;

namespace NextflowRunnerClient.Components;

public partial class PipelineParamText
{
    [Parameter]
    public ViewParam ViewParam { get; set; } = new ViewParam();
}