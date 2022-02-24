
using Microsoft.Azure.Management.Fluent;
using Microsoft.Azure.Management.ResourceManager.Fluent;
using Microsoft.Rest.Azure;
using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Azure.Management.ContainerInstance.Fluent.Models;

string resourceGroupName = "demo-aci";
string containerGroupName = "aci-task-demo";
string containerImage = "mcr.microsoft.com/azuredocs/aci-wordcount";

//var credentials = SdkContext.AzureCredentialsFactory.FromDevice("94c9fc18-8343-4994-b49d-7f4caf0a1ebe", "72f988bf-86f1-41af-91ab-2d7cd011db47", AzureEnvironment.AzureGlobalCloud);

IAzure azure = Azure.Authenticate("my.azureauth").WithDefaultSubscription();

Dictionary<string, string> envVars = new Dictionary<string, string>
            {
                { "NumWords", "5" },
                { "MinLength", "8" }
            };

Console.WriteLine($"\nCreating container group '{containerGroupName}'");

// Get the resource group's region
IResourceGroup resGroup = azure.ResourceGroups.GetByName(resourceGroupName);
Region azureRegion = resGroup.Region;

// Create the container group
var containerGroup = azure.ContainerGroups.Define(containerGroupName)
    .WithRegion(azureRegion)
    .WithExistingResourceGroup(resourceGroupName)
    .WithLinux()
    .WithPublicImageRegistryOnly()
    .WithoutVolume()
    .DefineContainerInstance(containerGroupName + "-1")
        .WithImage(containerImage)
        .WithExternalTcpPort(80)
        .WithCpuCoreCount(1.0)
        .WithMemorySizeInGB(1)
        .WithEnvironmentVariables(envVars)
        .Attach()
    .WithDnsPrefix(containerGroupName)
    .WithRestartPolicy(ContainerGroupRestartPolicy.Never)
    .Create();

// Print the container's logs
Console.WriteLine($"Logs for container '{containerGroupName}-1':");
Console.WriteLine(containerGroup.GetLogContent(containerGroupName + "-1"));