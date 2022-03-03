param location string
param tagVersion string
param nfRunnerAPIAppPlanName string
param nfRunnerAPIAppName string
param storageAccountName string
@secure()
param storagePassphrase string
@secure()
param storageSASToken string
param functionAppUrl string

//param expireTime string = dateTimeAdd(utcNow('u'), 'P1Y')

@allowed([
  'nonprod'
  'prod'
])
param environmentType string

// App Settings
@secure()
param sqlConnection string

var appServicePlanSkuName = (environmentType == 'prod') ? 'P2_v2' : 'B1'
var appServicePlanTierName = (environmentType == 'prod') ? 'PremiumV2' : 'Basic'

var tagName = split(tagVersion, ':')[0]
var tagValue = split(tagVersion, ':')[1]

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: nfRunnerAPIAppPlanName
  location: location
  tags: {
    '${tagName}': tagValue
  }
  sku: {
    name: appServicePlanSkuName
    tier: appServicePlanTierName
  }
  kind: 'Linux'
  properties: {
    reserved: true
  }
}

resource appServiceApp 'Microsoft.Web/sites@2021-01-15' = {
  name: nfRunnerAPIAppName
  location: location
  tags: {
    '${tagName}': tagValue
  }
  properties: {
    serverFarmId: appServicePlan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|6.0'
      connectionStrings: [
        {
          name: 'DefaultConnection'
          connectionString: sqlConnection
          type: 'SQLAzure'
        }
      ]
      appSettings: [        
        {
          name: 'AzureStorage__AZURE_STORAGE_ACCOUNTNAME'
          value: storageAccountName
        }
        {
          name: 'AzureStorage__AZURE_STORAGE_KEY'
          value: storagePassphrase
        }
        {
          name: 'AzureStorage__AZURE_STORAGE_SAS'
          value: storageSASToken
        }
        {
          name: 'OrchestratorClientOptions__WeblogUrl'
          value: '${functionAppUrl}/api/WeblogTracer'
        }
        {
          name: 'OrchestratorClientOptions__HttpStartUrl'
          value: '${functionAppUrl}/api/ContainerManager_HttpStart'
        }
      ]
    }
  }
}

output appServiceAppName string = appServiceApp.name
