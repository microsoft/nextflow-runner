namespace NextflowRunnerAPI.Models
{
    public record PipelineParam
    {
        public int PipelineParamId { get; set; }
        public int PipelineId { get; set; }
        public string ParamName { get; set; }
        public object ParamType { get; set; }
    }
}
