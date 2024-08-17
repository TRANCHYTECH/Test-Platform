param location string = resourceGroup().location
param keyVaultName string
param tenantId string = tenant().tenantId
param tenantObjectId string

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
    enableSoftDelete: false
    tenantId: tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
    accessPolicies: [
      {
        objectId: tenantObjectId
        tenantId: tenantId
        permissions: {
          secrets: [
            'all'
          ]
          certificates: [
            'all'
          ]
          keys: [
            'all'
          ]
        }
      }
    ]
  }
}
