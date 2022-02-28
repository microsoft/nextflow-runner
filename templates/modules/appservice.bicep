param location string = resourceGroup().location
param nfRunnerAPIAppPlanName string
param nfRunnerAPIAppName string
param weblogPostUrl string
param storageAccountName string
@secure()
param storagePassphrase string

param expireTime string = dateTimeAdd(utcNow('u'), 'P1Y')

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

// var sasTokenProps = {
//   canonicalizedResource: '/blob/${storageAccountName}/nextflow'
//   signedResource: 'c'
//   signedProtocol: 'https'
//   signedPermission: 'w'
//   signedServices: 'b'
//   signedExpiry: expireTime
// }
// var storageSASToken = listServiceSAS(storageAccountName, '2021-06-01', sasTokenProps).serviceSasToken
var storageSASToken = 'placeholder'

resource appServicePlan 'Microsoft.Web/serverfarms@2021-01-15' = {
  name: nfRunnerAPIAppPlanName
  location: location
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
          name: 'SSHConnection__WEBLOG_URL'
          value: weblogPostUrl
        }
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
      ]
    }
  }
}

output appServiceAppName string = appServiceApp.name
