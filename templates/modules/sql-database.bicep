param sqlDatabaseName string
param sqlServerName string
param location string = resourceGroup().location
param sqlAdminUserName string
@secure()
param sqlAdminPassword string

@allowed([
  'nonprod'
  'prod'
])
param environmentType string

var databaseSkuName = (environmentType == 'prod') ? 'S1' : 'Basic'
var databaseTierName = (environmentType == 'prod') ? 'Standard' : 'Basic'

resource nfRunnerDBServer 'Microsoft.Sql/servers@2021-02-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminUserName
    administratorLoginPassword:sqlAdminPassword
    version: '12.0'
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'    
  }
  resource allowAzure 'firewallRules' = {
    name: 'AllowAllWindowsAzureIps'
    properties: {
      startIpAddress: '0.0.0.0'
      endIpAddress: '0.0.0.0'
    }
  }
}

resource nfRunnerSQLDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  parent: nfRunnerDBServer
  name: sqlDatabaseName
  location: location
  sku: {
    name: databaseSkuName
    tier: databaseTierName
    capacity: 5
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
    isLedgerOn: false
  }
}

output sqlUserName string = sqlAdminUserName
output sqlServerName string = nfRunnerDBServer.name
output sqlServerFQDN string = nfRunnerDBServer.properties.fullyQualifiedDomainName
output sqlDbName string = nfRunnerSQLDatabase.name
