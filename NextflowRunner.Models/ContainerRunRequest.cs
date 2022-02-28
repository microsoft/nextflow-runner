namespace NextflowRunner.Models;

public class ContainerRunRequest
{
    public string RunName { get; set; }
    public string Command { get; set; } = "nextflow run nextflow-io/hello";
    public string ContainerImage { get; set; } = "nextflow/nextflow:21.10.6";
    // todo: provide params for environment variables to the container
    public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    // storage account config will be built into image
    // other params specific to pipelines come from ui through this request
}