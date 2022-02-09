namespace NextflowRunnerAPI.Models
{
    public record PipelineRun
    {
        public int PipelineRunId { get; set; }
        public int PipelineId { get; set; }
        public string NextflowRunCommand { get; set; }
        public DateTime RunDateTime { get; set; }
        public string Status { get; set; }
    }
}
