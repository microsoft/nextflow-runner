| Backend | Frontend |
| ------- | -------- |
| [![Nextflow-Runner API](https://github.com/microsoft/nextflow-runner/actions/workflows/main.yml/badge.svg)](https://github.com/microsoft/nextflow-runner/actions/workflows/main.yml) | [![Client App](https://github.com/microsoft/nextflow-runner/actions/workflows/azure-static-web-apps-happy-ocean-0f6450210.yml/badge.svg)](https://github.com/microsoft/nextflow-runner/actions/workflows/azure-static-web-apps-happy-ocean-0f6450210.yml) |

# Introduction
| :exclamation:  This is a starter-kit. There are several conditions that must be addressed before running in a production or enterprise scenario. Use this as a starting place and ensure you meet your organizations' security, performance, exception handling, monitoring and deployment concerns.  |
|-----------------------------------------|

Please submit a new issue to address any concerns you might find, or consider making a pull request.

## What?
A lightweight application designed to allow users to run Nextflow jobs without setting up Nextflow on Azure. It allows users to bring their own data, select from a list of preconfigured pipelines and execute a pipeline.

All output is stored in an Azure storage account and can be downloaded for further downstream processing.

## Why?
Nextflow is a commonly used workflow manager in the scientific community. By providing  a sandox type environment for users, users can quickly validate their data and generate data for further analysis without having to learn or configure nextflow jobs.

### Bio-informaticians
* Quickly get started with Nextflow on Azure.
* BYOD - Use preconfigured pipelines to experiment common pipelines with your data.
* All intermediate results are automatically tracked
* Output is available in Azure Storage for review
* Open-source community with available samples for most common pipelines.


### Operations teams
* Nothing to deploy
* Use Azure auto-scaling compute
* Use Azure Storage for maximum throughput and scalability and tighter access control using ACLs.
* Use Azure PaaS services to automate deployment and monitor progress.


# Getting started

## Nextflow Cloud Runner
Run a Nextflow pipeline in the cloud

![Diagram of cloud resources deployed by project](./docs/imgs/nextflow-runner.png)

## Requirements
This project has three parts, a database project, an Minimal API project, and a Blazor UI application. We've also included the necessary bicept templates to create and deploy the resources on Azure.

## Running locally
You can download and run the projects locally, .NET 6.0 is required.

## Running on Azure
You can use the provided bicep templates to deploy and run on Azure.
- Pre-reqs
- Backend provisioning with GitHub workflow
- Frontend provisioning 


## Contributing

This project welcomes contributions and suggestions.  Most contributions require you to agree to a
Contributor License Agreement (CLA) declaring that you have the right to, and actually do, grant us
the rights to use your contribution. For details, visit https://cla.opensource.microsoft.com.

When you submit a pull request, a CLA bot will automatically determine whether you need to provide
a CLA and decorate the PR appropriately (e.g., status check, comment). Simply follow the instructions
provided by the bot. You will only need to do this once across all repos using our CLA.

This project has adopted the [Microsoft Open Source Code of Conduct](https://opensource.microsoft.com/codeofconduct/).
For more information see the [Code of Conduct FAQ](https://opensource.microsoft.com/codeofconduct/faq/) or
contact [opencode@microsoft.com](mailto:opencode@microsoft.com) with any additional questions or comments.

## Trademarks

This project may contain trademarks or logos for projects, products, or services. Authorized use of Microsoft 
trademarks or logos is subject to and must follow 
[Microsoft's Trademark & Brand Guidelines](https://www.microsoft.com/en-us/legal/intellectualproperty/trademarks/usage/general).
Use of Microsoft trademarks or logos in modified versions of this project must not cause confusion or imply Microsoft sponsorship.
Any use of third-party trademarks or logos are subject to those third-party's policies.
