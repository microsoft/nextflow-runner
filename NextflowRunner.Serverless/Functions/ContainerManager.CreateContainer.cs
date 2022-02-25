using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NextflowRunner.Models;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_CreateContainer")]
    public string CreateContainer([ActivityTrigger] ContainerRunRequest req)
    {
        var containerGroupName = req.RunName + "-containergroup";
        var containerGroup = _azure.ContainerGroups.Define(containerGroupName)
            .WithRegion(_azure.ResourceGroups.GetByName(_containerConfig.ResourceGroupName).Region)
            .WithExistingResourceGroup(_containerConfig.ResourceGroupName)
            .WithLinux()
            .WithPublicImageRegistryOnly()
            .WithoutVolume()
            .DefineContainerInstance(req.RunName + "-container")
                .WithImage(req.ImageName) // todo: one image or many images? - do we derive from user input or config?
                // todo: configure container better for nextflow
                .WithExternalTcpPort(80)
                .WithCpuCoreCount(1.0)
                .WithMemorySizeInGB(1)
                .WithStartingCommandLine(req.Command)
                .Attach()
            .WithDnsPrefix(containerGroupName)
            .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
            .Create();

        return containerGroup.Id;
    }
}
