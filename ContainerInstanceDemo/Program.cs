using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Management.ContainerInstance;
using Microsoft.Azure.Management.ContainerInstance.Models;
using Microsoft.Rest;

string resourceGroupName = "demo-aci";
string containerGroupName = "aci-task-demo";
string containerImage = "nextflow/nextflow:21.10.6";
string tenantId = "<your-tenant-id>";
string subscriptionId = "<your-subscription-id>";

var defaultCredential = new DefaultAzureCredential();
var defaultToken = defaultCredential.GetToken(new TokenRequestContext(new[] { "https://management.azure.com/.default" }, tenantId: tenantId)).Token;
var creds = new TokenCredentials(defaultToken);

var envVars = new List<EnvironmentVariable>
{
    new EnvironmentVariable{ Name = "STORAGE_NAME", Value = "hli6aqxtwj2yybatchsa"},
    new EnvironmentVariable{ Name = "STORAGE_KEY", Value = ""},
    new EnvironmentVariable{ Name = "BATCH_REGION", Value = "centralus"},
    new EnvironmentVariable{ Name = "BATCH_ACCOUNT", Value = "hli6aqxtwj2yybatch"},
    new EnvironmentVariable{ Name = "BATCH_KEY", Value = ""}
};
    
Console.WriteLine($"\nCreating container group '{containerGroupName}'");
ContainerInstanceManagementClient client = new ContainerInstanceManagementClient(creds);
client.SubscriptionId = subscriptionId;

var container = new Container(
    $"{containerGroupName}-1",
    containerImage,
    new ResourceRequirements { Requests = new ResourceRequests(1.5, 1.0)},
    new[] { "nextflow", "run", "nextflow-io/hello" },
    environmentVariables: envVars    
    );

var group = new ContainerGroup()
{
    OsType = "Linux",
    Location = "centralus",
    RestartPolicy = "Never",
    Containers = new[] {container}

};

Console.WriteLine($"\nCreating container group '{containerGroupName}'");

client.SubscriptionId = subscriptionId;
client.ContainerGroups.BeginCreateOrUpdate(resourceGroupName, containerGroupName, group);
var logs = client.Containers.ListLogs(resourceGroupName, containerGroupName, $"{containerGroupName}-1");

Console.WriteLine(logs.Content);
