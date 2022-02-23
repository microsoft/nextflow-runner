param location string = resourceGroup().location
param nfRunnerAPIAppPlanName string
param nfRunnerAPIAppName string
param vmAdminUserName string
param vmHostName string
param weblogPostUrl string


@allowed([
  'nonprod'
  'prod'
])
param environmentType string

// App Settings
@secure()
param sqlConnection string

@secure()
param vmAdminPassword string

var appServicePlanSkuName = (environmentType == 'prod') ? 'P2_v2' : 'B1'
var appServicePlanTierName = (environmentType == 'prod') ? 'PremiumV2' : 'Basic'

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
          name: 'SSHConnection__VM_ADMIN_USERNAME'
          value: vmAdminUserName
        }
        {
          name: 'SSHConnection__VM_ADMIN_PASSWORD'
          value: vmAdminPassword
        }
        {
          name: 'SSHConnection__VM_HOSTNAME'
          value: vmHostName
        }
        {
          name: 'SSHConnection__WEBLOG_URL'
          value: weblogPostUrl
        }
      ]
    }
  }
}

output appServiceAppName string = appServiceApp.name
