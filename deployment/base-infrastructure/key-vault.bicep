param location string = resourceGroup().location
param keyVaultName string
param tenantId string = tenant().tenantId

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' = {
  name: keyVaultName
  location: location
  properties: {
    enableRbacAuthorization: true
    publicNetworkAccess: 'Enabled'
    tenantId: tenantId
    sku: {
      name: 'standard'
      family: 'A'
    }
  }
}
