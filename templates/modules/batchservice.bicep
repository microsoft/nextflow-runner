param batchAccountName string
param storageAccountName string
param keyvaultName string
param location string
param storageContainerName string = 'nextflow'

resource batchStorage 'Microsoft.Storage/storageAccounts@2021-06-01' = {
  name: storageAccountName
  location: location
  tags: {
    'ObjectName': batchAccountName
  }
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
}

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2021-08-01' = {
  name: '${batchStorage.name}/default'
  properties: {
    cors: {
      corsRules: [
        {
          allowedHeaders: [
            '*'
          ]
          allowedMethods: [
            'GET'
            'HEAD'
            'MERGE'
            'OPTIONS'
            'POST'
            'PUT'
          ]
          allowedOrigins: [
            '*'
          ]
          exposedHeaders: [
            '*'
          ]
          maxAgeInSeconds: 3600
        }
      ]
    }
  }
}

resource storageContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2021-08-01' = {
  name: '${storageAccountName}/default/${storageContainerName}'
}

// resource storagePolicy 'Microsoft.Storage/storageAccounts/managementPolicies@2021-08-01' = {
//   name: '${batchStorage.name}/AutoDelete48hours'
//   properties: {
//     policy: {
//       rules: [
//         {
//           enabled: true
//           name: 'AutoDelete48hours'
//           type: 'Lifecycle'
//           definition: {
//             actions: {
//               baseBlob: {
//                 delete: {
//                   daysAfterModificationGreaterThan: 2
//                 }
//               }
//             }
//             filters: {
//               blobTypes: [
//                 'blockBlob'
//               ]
//               prefixMatch: [
//                 storageContainer.name
//               ]
//             }
//           }
//         }
//       ]
//     }
//   }
// }

resource batchService 'Microsoft.Batch/batchAccounts@2021-06-01' = {
  name: batchAccountName
  location: location
  properties: {
    autoStorage: {
      storageAccountId: batchStorage.id
    }
  }
  tags: {
    'ObjectName': batchAccountName
  }
}

resource batchKey 'Microsoft.KeyVault/vaults/secrets@2021-10-01' = {
  name: '${keyvaultName}/batch-key'
  properties: {
    value: batchService.listKeys().primary
  }
}

resource storageKey 'Microsoft.KeyVault/vaults/secrets@2021-10-01' = {
  name: '${keyvaultName}/storage-key'
  properties: {
    value: batchStorage.listKeys().keys[0].value
  }
}

output batchAccountName string = batchService.name
output storageAccountName string = batchStorage.name
