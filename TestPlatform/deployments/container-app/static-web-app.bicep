param appName string
param location string = 'Central US'

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: 'test-master-services-access-identity'
}

resource staticSite 'Microsoft.Web/staticSites@2022-09-01' = {
  name: appName
  location: location
  // identity: {
  //   type: 'UserAssigned'
  //   userAssignedIdentities: {
  //     '${uai.id}': {}
  //   }
  // }
  properties: {
  }
  sku: {
    tier: 'Free'
    name: 'Free'
  }
}
