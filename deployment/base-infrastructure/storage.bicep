param location string = resourceGroup().location
param accountName string
param document string = 'test-runner.key'

param userAssignedIdentity string

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' existing = {
  name: userAssignedIdentity
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: accountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    minimumTlsVersion: 'TLS1_2'
  }
}

resource deploymentScript 'Microsoft.Resources/deploymentScripts@2023-08-01' = {
  name: 'create-data-protection-keys-blob'
  location: location
  kind: 'AzurePowerShell'
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uai.id}': {
      }
    }
  }
  dependsOn: [
    storageAccount
  ]
  properties: {
    azPowerShellVersion: '5.4'
    scriptContent: loadTextContent('create-test-run-protection-key-blob.ps1')
    cleanupPreference: 'Always'
    retentionInterval: 'PT4H'
    arguments: '-ResourceGroupName ${resourceGroup().name} -StorageAccountName ${accountName} -IndexDocument ${document}'
  }
}
