using NextflowRunnerClient.Services;

namespace NextflowRunnerClient.Models;

public class ViewParam : PipelineParam
{
    public ViewParam() { }
    public ViewParam(PipelineParam pipelineParam)
    {
        PipelineId = pipelineParam.PipelineId;
        PipelineParamId = pipelineParam.PipelineParamId;
        ParamName = pipelineParam.ParamName;
        ParamType = pipelineParam.ParamType;
        ParamExample = pipelineParam.ParamExample;
        DefaultValue = pipelineParam.DefaultValue;
    }
    public string Value { get; set; } = null;
}
