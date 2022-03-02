using Microsoft.Azure.Management.ContainerInstance;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Threading.Tasks;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_DestroyContainer")]
    public async Task DestroyContainer([ActivityTrigger] string containerGroupName)
    {
        await _containerInstanceClient.ContainerGroups.DeleteAsync(_containerConfig.ResourceGroupName, containerGroupName);
    }
}
