using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace NextflowRunner.Serverless.Functions;

public partial class ContainerManager
{
    [FunctionName("ContainerManager_DestroyContainer")]
    public void DestroyContainer([ActivityTrigger] string containerGroupId)
    {
        _azure.ContainerGroups.DeleteById(containerGroupId);
    }
}
