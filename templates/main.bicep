param prefix string = 'nf-runner-${uniqueString(resourceGroup().id)}'
param tagVersion string = 'nf-runner-version:v1.2.0'
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
param keyVaultName string = substring('${replace(prefix, '-', '')}-kv',0,24)

@description('A shared passphrase that allow users to upload files in the UI')
@secure()
param storagePassphrase string

@secure()
param sqlAdminPassword string

@secure()
param keyvaultSpnObjectId string

@allowed([
  'nonprod'
  'prod'
])
param environmentType string

var tenantId = subscription().tenantId
var roleName = 'Key Vault Secrets Officer'

var roleIdMapping = {
  'Key Vault Administrator': '00482a5a-887f-4fb3-b363-3b7fe8e74483'
  'Key Vault Certificates Officer': 'a4417e6f-fecd-4de8-b567-7b0420556985'
  'Key Vault Crypto Officer': '14b46e9e-c2b7-41b4-b07b-48a6ebf60603'
  'Key Vault Crypto Service Encryption User': 'e147488a-f6f5-4113-8e2d-b22465e65bf6'
  'Key Vault Crypto User': '12338af0-0e69-4776-bea7-57ae8d297424'
  'Key Vault Reader': '21090545-7ca7-4776-b22c-e363652d74d2'
  'Key Vault Secrets Officer': 'b86a8fe4-44ce-4948-aee5-eccb2c155cd7'
  'Key Vault Secrets User': '4633458b-17de-408a-b874-0445c86b69e6'
}

var tagKvp = split(tagVersion, ':')

resource keyvault 'Microsoft.KeyVault/vaults@2021-04-01-preview' = {
  name: keyVaultName
  location: location
  tags: {
    '${tagKvp[0]}': tagKvp[1]
  }
  properties: {
    sku: {
      name: 'standard'
      family: 'A'
    }
    tenantId: tenantId
    enabledForTemplateDeployment: true
    enableRbacAuthorization: true
    networkAcls: {
      defaultAction: 'Allow'
      bypass: 'AzureServices'
    }
  }
}

resource kvRoleAssignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(roleIdMapping[roleName],keyvaultSpnObjectId,keyvault.id)
  scope: keyvault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', roleIdMapping[roleName])
    principalId: keyvaultSpnObjectId
    principalType: 'ServicePrincipal'
  }
}

module sqlDatabase 'modules/sql-database.bicep' = {
  name: 'hackAPI-database'
  params: {
    tagVersion: tagVersion
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
    tagVersion: tagVersion
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
    tagVersion: tagVersion
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
  params: {
    tagVersion: tagVersion
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
