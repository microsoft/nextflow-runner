using Microsoft.AspNetCore.Components;
using NextflowRunnerClient.Models;

namespace NextflowRunnerClient.Components;

public partial class PipelineParam
{
    [Parameter]
    public ViewParam Param { get; set; } = new ();

    protected override void OnInitialized()
    {
        Param.Value = Param.ParamExample;
    }
}