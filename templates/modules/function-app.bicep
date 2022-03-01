param location string = resourceGroup().location
param functionAppName string
param functionStorageAccountName string
param batchStorageAccountName string
@secure()
param batchStorageAccountKey string 
param batchAccountName string
@secure()
param batchAccountKey string

@secure()
param servicePrincipalClientId string
@secure()
param servicePrincipalClientSecret string
@secure()
param sqlConnection string

resource functionStorageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: functionStorageAccountName
  location: location
  tags: {
    'ObjectName': functionAppName
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource functionApp 'Microsoft.Web/sites@2020-12-01' = {
  name: functionAppName
  location: location
  kind: 'functionapp'
  properties: {
    siteConfig: {
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorageAccount.id, functionStorageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${functionStorageAccount.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(functionStorageAccount.id, functionStorageAccount.apiVersion).keys[0].value}'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~4'
        }
        {
          name: 'ContainerConfiguration:ClientId'
          value: servicePrincipalClientId
        }
        {
          name: 'ContainerConfiguration:ClientSecret'
          value: servicePrincipalClientSecret
        }
        {
          name: 'ContainerConfiguration:TenantId'
          value: tenant().tenantId
        }
        {
          name: 'ContainerConfiguration:SubscriptionId'
          value: subscription().subscriptionId
        }
        {
          name: 'ContainerConfiguration:ResourceGroupName'
          value: resourceGroup().name
        }
        {
          name: 'ConnectionStrings:DefaultConnection'
          value: sqlConnection
        }
        {
          name: 'ContainerConfiguration:StorageName'
          value: batchStorageAccountName
        }
        {
          name: 'ContainerConfiguration:StorageKey'
          value: batchStorageAccountKey
        }
        {
          name: 'ContainerConfiguration:BatchRegion'
          value: location
        }
        {
          name: 'ContainerConfiguration:BatchAccountName'
          value: batchAccountName
        }
        {
          name: 'ContainerConfiguration:BatchKey'
          value: batchAccountKey
        }
      ]
    }
    httpsOnly: true
  }
}

output functionAppUrl string = functionApp.properties.defaultHostName
output functionAppName string = functionApp.name
