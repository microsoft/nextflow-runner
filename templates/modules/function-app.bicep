param location string
param tagVersion string
param functionAppName string
param appServicePlanName string
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

var appInsightsName = '${functionAppName}-ai'
var tagName = split(tagVersion, ':')[0]
var tagValue = split(tagVersion, ':')[1]

resource functionStorageAccount 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: functionStorageAccountName
  location: location
  tags: {
    ObjectName: functionAppName
    '${tagName}': tagValue
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource appInsights 'Microsoft.Insights/components@2020-02-02' = {
  name: appInsightsName
  location: location
  tags: {
    ObjectName: functionAppName
    '${tagName}': tagValue
  }
  kind: 'web'
  properties: {
    Application_Type: 'web'
    publicNetworkAccessForIngestion: 'Enabled'
    publicNetworkAccessForQuery: 'Enabled'
  }
}

resource plan 'Microsoft.Web/serverfarms@2020-12-01' = {
  name: appServicePlanName
  location: location
  tags: {
    '${tagName}': tagValue
  }
  kind: 'functionapp'
  sku: {
    name: 'Y1'
  }
  properties: {}
}

resource functionApp 'Microsoft.Web/sites@2020-12-01' = {
  name: functionAppName
  location: location
  tags: {
    '${tagName}': tagValue
  }
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
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: appInsights.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: 'InstrumentationKey=${appInsights.properties.InstrumentationKey}'
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

output functionAppUrl string = 'https://${functionApp.properties.defaultHostName}'
output functionAppName string = functionApp.name
