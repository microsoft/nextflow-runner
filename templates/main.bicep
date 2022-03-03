param prefix string = 'nf-runner-${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location
param sqlDatabaseName string = 'nf-runnerDB'
param sqlServerName string = '${prefix}-sqlserver'
param sqlAdminUserName string = 'nf-runner-admin'
param nfRunnerAPIAppPlanName string = '${prefix}-appPlan'
param nfRunnerAPIAppName string = '${prefix}-api'
param nfRunnerFunctionAppName string = '${prefix}-serverless'
param nfRunnerFunctionAppStorageName string = substring('${replace(prefix, '-', '')}funcsa',0,24)
param batchAccountName string = '${replace(prefix, '-', '')}batch'
param batchStorageName string = substring('${replace(prefix, '-', '')}batchsa',0,24)

@description('An existing keyvault with secrets for container instance and API apps')
param keyVaultName string = 'nfrunnerkv'

@description('A shared passphrase that allow users to upload files in the UI')
@secure()
param storagePassphrase string

@secure()
param sqlAdminPassword string

@allowed([
  'nonprod'
  'prod'
])
param environmentType string

resource keyvault 'Microsoft.KeyVault/vaults@2021-10-01' existing = {
  name: keyVaultName
}

module sqlDatabase 'modules/sql-database.bicep' = {
  name: 'hackAPI-database'
  params: {
    environmentType: environmentType
    location: location
    sqlAdminUserName: sqlAdminUserName
    sqlAdminPassword: sqlAdminPassword
    sqlDatabaseName: sqlDatabaseName
    sqlServerName: sqlServerName
  }
}

module batch 'modules/batchservice.bicep' = {
  name: 'batch-account'
  params: {
    keyvaultName: keyVaultName
    location: location
    batchAccountName: batchAccountName
    storageAccountName: batchStorageName    
  }
}

var sqlConn = 'Server=tcp:${sqlDatabase.outputs.sqlServerFQDN},1433;Initial Catalog=${sqlDatabase.outputs.sqlDbName};Persist Security Info=False;User ID=${sqlAdminUserName};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'

module functionApp 'modules/function-app.bicep' = {
  name: 'nextflow-runner-serverless'
  params: {
    location: location
    functionAppName: nfRunnerFunctionAppName
    functionStorageAccountName: nfRunnerFunctionAppStorageName
    batchStorageAccountName: batch.outputs.storageAccountName
    batchStorageAccountKey: keyvault.getSecret('storage-key')
    batchAccountName: batch.outputs.batchAccountName
    batchAccountKey: keyvault.getSecret('batch-key')
    servicePrincipalClientId: keyvault.getSecret('SP-AzureFunction-ClientId')
    servicePrincipalClientSecret: keyvault.getSecret('SP-AzureFunction-ClientSecret')
    sqlConnection: sqlConn
  }
}

module appService 'modules/appservice.bicep' = {
  name: 'nextflow-runner-appservice'
  dependsOn: [
    batch
  ]
  params: {
    environmentType: environmentType
    location: location
    nfRunnerAPIAppName: nfRunnerAPIAppName
    nfRunnerAPIAppPlanName: nfRunnerAPIAppPlanName
    sqlConnection: sqlConn
    storageAccountName: batch.outputs.batchAccountName
    storagePassphrase: storagePassphrase
    storageSASToken: keyvault.getSecret('storage-sas-token')
    functionAppUrl: functionApp.outputs.functionAppUrl
  }
}

output appServiceAppName string = appService.outputs.appServiceAppName
output functionAppName string = functionApp.outputs.functionAppName
output sqlServerFQDN string = sqlDatabase.outputs.sqlServerFQDN
output sqlServerName string =sqlDatabase.outputs.sqlServerName
output sqlDbName string = sqlDatabase.outputs.sqlDbName
output sqlUserName string = sqlDatabase.outputs.sqlUserName
output batchAccountName string = batch.outputs.batchAccountName
