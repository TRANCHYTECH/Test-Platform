param location string = 'southeastasia'
param accountName string = 'stvgtmgeneraldevsa001'
param document string = 'test-runner.key'

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: 'test-master-services-access-identity'
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: accountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {}
}

resource deploymentScript 'Microsoft.Resources/deploymentScripts@2020-10-01' = {
  name: 'create-data-protection-keys-blob'
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uai.id}': {}
    }
  }
  kind: 'AzurePowerShell'
  properties: {
    azPowerShellVersion: '5.4'
    scriptContent: loadTextContent('scripts/create-test-run-protection-key-blob.ps1')
    cleanupPreference: 'Always'
    retentionInterval: 'PT4H'
    arguments: '-ResourceGroupName ${resourceGroup().name} -StorageAccountName ${accountName} -IndexDocument ${document}'
  }
  dependsOn: [
    storageAccount
  ]
}
