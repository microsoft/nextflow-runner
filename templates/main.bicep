param prefix string = '${uniqueString(resourceGroup().id)}'
param location string = resourceGroup().location
param sqlDatabaseName string = 'nf-runnerDB'
param sqlServerName string = '${prefix}-${sqlDatabaseName}-server'
param sqlAdminUserName string = 'nf-runner-admin'
param nfRunnerAPIAppPlanName string = '${prefix}-nfrunner-plan'
param nfRunnerAPIAppName string = '${prefix}-nf-runner-api'
param batchAccountName string = '${prefix}batch'
param batchStorageName string = '${prefix}batchsa'
param vmAdminUserName string = 'azureuser'
param vmHostName string = '40.83.22.113'

@secure()
param sqlAdminPassword string

@secure()
param vmAdminPassword string

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
    location: location
    batchAccountName: batchAccountName
    storageAccountName: batchStorageName
  }
}

module appService 'modules/appservice.bicep' = {
  name: 'nextflow-runner-appservice'
  params: {
    environmentType: environmentType
    location: location
    nfRunnerAPIAppName: nfRunnerAPIAppName
    nfRunnerAPIAppPlanName: nfRunnerAPIAppPlanName
    sqlConnection: 'Server=tcp:${sqlDatabase.outputs.sqlServerFQDN},1433;Initial Catalog=${sqlDatabase.outputs.sqlDbName};Persist Security Info=False;User ID=${sqlAdminUserName};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
    vmAdminUserName: vmAdminUserName
    vmAdminPassword: vmAdminPassword
    vmHostName: vmHostName
  }
}

module clientApp 'modules/staticsite.bicep' = {
  name: 'nextflow-runner-client'
  params: {
    swaSiteName: 'name'
    location: location
    repositoryToken: repositoryToken
  }
}

output appServiceAppName string = appService.outputs.appServiceAppName
output sqlServerFQDN string = sqlDatabase.outputs.sqlServerFQDN
output sqlServerName string =sqlDatabase.outputs.sqlServerName
output sqlDbName string = sqlDatabase.outputs.sqlDbName
output sqlUserName string = sqlDatabase.outputs.sqlUserName
output batchAccountName string = batch.outputs.batchAccountName
