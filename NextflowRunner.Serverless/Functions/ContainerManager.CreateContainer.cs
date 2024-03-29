﻿using Microsoft.Azure.Management.ContainerInstance;
using Microsoft.Azure.Management.ContainerInstance.Models;
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

        var container = new Container(
            $"{containerGroupName}-container",
            req.ContainerImage,
            new ResourceRequirements { Requests = new ResourceRequests(1.5, 1.0) },
            req.Command.Split(' '),
            environmentVariables: _containerEnvVariables
        );

        var group = new ContainerGroup()
        {
            OsType = "Linux",
            Location = _containerConfig.BatchRegion,
            RestartPolicy = "Never",
            Containers = new[] { container }
        };

        var containerGroup = _containerInstanceClient.ContainerGroups.BeginCreateOrUpdate(_containerConfig.ResourceGroupName, containerGroupName, group);

        return containerGroup.Name;
    }
}
