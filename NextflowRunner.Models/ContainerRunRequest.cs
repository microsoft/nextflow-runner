namespace NextflowRunner.Models;

public class ContainerRunRequest
{
    public string RunName { get; set; }
    public string Command { get; set; }
    public string ImageName { get; set; }
}