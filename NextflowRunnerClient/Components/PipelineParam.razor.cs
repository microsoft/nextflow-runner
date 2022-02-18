using Microsoft.AspNetCore.Components;

namespace NextflowRunnerClient.Components;

public partial class PipelineParam
{
    [Parameter]
    public Services.PipelineParam Param { get; set; } = new Services.PipelineParam();

    [Parameter]
    public string ParamValue { get; set; } = "";

    public bool Valid { get; set; } = false;

    public void ValidateParam()
    {
        Valid = false;
        if (ParamValue.Length > 3)
        {
            Valid = true;
        }
    }
}