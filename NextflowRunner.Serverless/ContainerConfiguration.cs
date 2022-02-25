namespace NextflowRunner.Serverless;

public class ContainerConfiguration
{
    public string ResourceGroupName { get; set; } = "demo-aci";
    public string ContainerGroupName { get; set; } = "aci-task-demo";
    public string ContainerImage { get; set; } = "mcr.microsoft.com/azuredocs/aci-wordcount";
}