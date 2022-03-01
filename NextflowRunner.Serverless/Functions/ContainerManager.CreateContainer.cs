using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NextflowRunner.Models;
using System.Linq;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_CreateContainer")]
    public string CreateContainer([ActivityTrigger] ContainerRunRequest req)
    {
        var containerGroupName = req.RunName + "-containergroup";

        var keyValuePairs = req.Parameters.Select(kvp => kvp).ToList();

        keyValuePairs.AddRange(_containerEnvVariables.Select(kvp => kvp).ToList());

        var environmentVariables = keyValuePairs.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

        var containerWithCreate = _azure.ContainerGroups.Define(containerGroupName)
            .WithRegion(_azure.ResourceGroups.GetByName(_containerConfig.ResourceGroupName).Region)
            .WithExistingResourceGroup(_containerConfig.ResourceGroupName)
            .WithLinux()
            .WithPublicImageRegistryOnly()
            .WithoutVolume()
            .DefineContainerInstance(req.RunName + "-container")
                .WithImage(req.ContainerImage)
                .WithExternalTcpPort(80)
                .WithCpuCoreCount(1.0)
                .WithMemorySizeInGB(1)
                .WithEnvironmentVariables(environmentVariables);

        if (!string.IsNullOrWhiteSpace(req.Command))
            containerWithCreate
                .WithStartingCommandLine(req.Command);

        var containerGroupWithCreate = containerWithCreate
            .Attach()
            .WithDnsPrefix(containerGroupName)
            .WithRestartPolicy(ContainerGroupRestartPolicy.Never);

        var containerGroup = containerGroupWithCreate.Create();

        return containerGroup.Id;
    }
}
