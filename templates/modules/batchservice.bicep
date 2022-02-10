param batchAccountName string
param storageAccountName string
param location string

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

output batchAccountName string = batchService.name
