namespace NextflowRunner.API;

public class OrchestrationOptions
{
    internal const string ConfigSection = "OrchestratorClientOptions";
    public string WeblogUrl { get; set; } = string.Empty;
    public string HttpStartUrl { get; set; } = string.Empty;
}