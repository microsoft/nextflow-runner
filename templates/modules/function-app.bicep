param location string = resourceGroup().location
param functionAppName string
param storageAccountName string

resource storageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' existing = {
  name: storageAccountName
}

// todo: add more environment variables

resource functionApp 'Microsoft.Web/sites@2020-12-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storageAccount.id, storageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
      ]
    }
    httpsOnly: true
  }
}

// todo: make one for http start, orchestrator, create container, destroy container, and weblog tracer
resource functionHttpStart 'Microsoft.Web/sites/functions@2020-12-01' = {
  name: '${functionApp.name}/containermanager.httpstart'
  properties: {
    config: {
      disabled: false
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'in'
          authLevel: 'function'
          methods: [
            'get'
          ]
        }
        {
          name: '$return'
          type: 'http'
          direction: 'out'
        }
      ]
    }
    files: {
      'ContainerManager.HttpStart.cs': loadTextContent('../../NextflowRunner.Serverless/Functions/ContainerManager.HttpStart.cs')
    }
  }
}

resource functionOrchestrator 'Microsoft.Web/sites/functions@2020-12-01' = {
  name: '${functionApp.name}/containermanager.orchestrator'
  properties: {
    config: {
      disabled: false
    }
    files: {
      'ContainerManager.Orchestrator.cs': loadTextContent('../../NextflowRunner.Serverless/Functions/ContainerManager.Orchestrator.cs')
    }
  }
}

resource functionCreateContainer 'Microsoft.Web/sites/functions@2020-12-01' = {
  name: '${functionApp.name}/containermanager.createcontainer'
  properties: {
    config: {
      disabled: false
    }
    files: {
      'ContainerManager.CreateContainer.cs': loadTextContent('../../NextflowRunner.Serverless/Functions/ContainerManager.CreateContainer.cs')
    }
  }
}

resource functionDestroyContainer 'Microsoft.Web/sites/functions@2020-12-01' = {
  name: '${functionApp.name}/containermanager.destroycontainer'
  properties: {
    config: {
      disabled: false
    }
    files: {
      'ContainerManager.DestroyContainer.cs': loadTextContent('../../NextflowRunner.Serverless/Functions/ContainerManager.DestroyContainer.cs')
    }
  }
}

resource functionWeblogTracer 'Microsoft.Web/sites/functions@2020-12-01' = {
  name: '${functionApp.name}/weblogtracer'
  properties: {
    config: {
      disabled: false
      bindings: [
        {
          name: 'req'
          type: 'httpTrigger'
          direction: 'in'
          authLevel: 'function'
          methods: [
            'get'
          ]
        }
        {
          name: '$return'
          type: 'http'
          direction: 'out'
        }
      ]
    }
    files: {
      'WeblogTracer.cs': loadTextContent('../../NextflowRunner.Serverless/Functions/WeblogTracer.cs')
    }
  }
}

// todo: output url for http start
// todo: output url for weblog tracer
