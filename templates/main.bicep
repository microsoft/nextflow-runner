param sqlDatabaseName string = 'nf-runnerDB'
param sqlServerName string = '${uniqueString(resourceGroup().id)}-${sqlDatabaseName}-server'
param location string = resourceGroup().location
param sqlAdminUserName string = 'nf-runner-admin'
param nfRunnerAPIAppPlanName string = 'nf-runner-apps-plan'
param nfRunnerAPIAppName string = 'nf-runner-api-${uniqueString(resourceGroup().id)}'

@secure()
param sqlAdminPassword string

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

module appService 'modules/appservice.bicep' = {
  name: 'hackAPI-appservice'
  params: {
    environmentType: environmentType
    location: location
    nfRunnerAPIAppName: nfRunnerAPIAppName
    nfRunnerAPIAppPlanName: nfRunnerAPIAppPlanName
    sqlConnection: 'Server=tcp:${sqlDatabase.outputs.sqlServerFQDN},1433;Initial Catalog=${sqlDatabase.outputs.sqlDbName};Persist Security Info=False;User ID=${sqlAdminUserName};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
  }
}

output appServiceAppName string = appService.outputs.appServiceAppName
output sqlServerFQDN string = sqlDatabase.outputs.sqlServerFQDN
output sqlServerName string =sqlDatabase.outputs.sqlServerName
output sqlDbName string = sqlDatabase.outputs.sqlDbName
output sqlUserName string = sqlDatabase.outputs.sqlUserName
