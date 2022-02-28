param prefix string = '${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location
param sqlDatabaseName string = 'nf-runnerDB'
param sqlServerName string = '${prefix}-${sqlDatabaseName}-server'
param sqlAdminUserName string = 'nf-runner-admin'
param nfRunnerAPIAppPlanName string = '${prefix}-nfrunner-plan'
param nfRunnerAPIAppName string = '${prefix}-nf-runner-api'
param nfRunnerClientAppName string = 'nextflowrunnerClient-${prefix}'
param batchAccountName string = '${prefix}batch'
param batchStorageName string = '${prefix}batchsa'
param acrName string = '${prefix}-acr'

@description('An existing keyvault with secrets for container instance and API apps')
param keyVaultName string = 'nfrunnerkv'

@secure()
param weblogPostUrl string

@secure()
param sqlAdminPassword string

@secure()
@description('Github token for static web app deployment')
param repositoryToken string

@allowed([
  'nonprod'
  'prod'
])
param environmentType string

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

module acr 'modules/container-registry.bicep' = {
  name: 'nf-runner-acr'
  params: {
    kvName: keyVaultName
    location: location
    acrName: acrName    
  }
}

resource keyvault 'Microsoft.KeyVault/vaults@2021-10-01' existing = {
  name: keyVaultName
}

module appService 'modules/appservice.bicep' = {
  name: 'nextflow-runner-appservice'
  params: {
    environmentType: environmentType
    location: location
    nfRunnerAPIAppName: nfRunnerAPIAppName
    nfRunnerAPIAppPlanName: nfRunnerAPIAppPlanName
    sqlConnection: 'Server=tcp:${sqlDatabase.outputs.sqlServerFQDN},1433;Initial Catalog=${sqlDatabase.outputs.sqlDbName};Persist Security Info=False;User ID=${sqlAdminUserName};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
    weblogPostUrl: weblogPostUrl
    storageAccountName: batchStorageName
    storageAccountKey: keyvault.getSecret('storage-key')
  }
}

module clientApp 'modules/staticsite.bicep' = {
  name: 'nextflow-runner-client'
  params: {
    swaSiteName: nfRunnerClientAppName
    location: location
    repositoryToken: repositoryToken
    repositoryBranch: 'main'
    repositoryUrl: 'https://github.com/microsoft/nextflow-runner'
  }
}

output appServiceAppName string = appService.outputs.appServiceAppName
output sqlServerFQDN string = sqlDatabase.outputs.sqlServerFQDN
output sqlServerName string =sqlDatabase.outputs.sqlServerName
output sqlDbName string = sqlDatabase.outputs.sqlDbName
output sqlUserName string = sqlDatabase.outputs.sqlUserName
output batchAccountName string = batch.outputs.batchAccountName
output acrLoginServer string = acr.outputs.acrLoginServer
