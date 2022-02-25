using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using NextflowRunner.Models;
using System.Collections.Generic;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_CreateContainer")]
    public string CreateContainer([ActivityTrigger] ExecutionRequest execReq)
    {
        var azure = Microsoft.Azure.Management.Fluent.Azure.Authenticate("my.azureauth")
            .WithDefaultSubscription();

        var containerGroup = azure.ContainerGroups.Define(_containerConfig.ContainerGroupName)
            .WithRegion(azure.ResourceGroups.GetByName(_containerConfig.ResourceGroupName).Region)
            .WithExistingResourceGroup(_containerConfig.ResourceGroupName)
            .WithLinux()
            .WithPublicImageRegistryOnly()
            .WithoutVolume()
            .DefineContainerInstance(_containerConfig.ContainerGroupName + "-1")
                .WithImage(_containerConfig.ContainerImage)
                .WithExternalTcpPort(80)
                .WithCpuCoreCount(1.0)
                .WithMemorySizeInGB(1)
                .WithEnvironmentVariables(new Dictionary<string, string>
                {
                    { "NumWords", "5" },
                    { "MinLength", "8" }
                })
                .Attach()
            .WithDnsPrefix(_containerConfig.ContainerGroupName)
            .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
            .Create();

        // todo: create container with image

        // execute command in container with weblog url

        return "Container created";
    }
}
