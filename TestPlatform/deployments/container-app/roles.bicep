var acrPullRole = resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')
var sbDataSenderRole = resourceId('Microsoft.Authorization/roleDefinitions', '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39')
var sbDataReceiverRole = resourceId('Microsoft.Authorization/roleDefinitions', '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0')
var storageAccountStorageBlobDataContributorRole = resourceId('Microsoft.Authorization/roleDefinitions', 'ba92f5b4-2d11-453d-a403-e96b0029c9fe')
var storageAccountContributorRole = subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '17d1049b-9a84-46fb-8f53-869881c3d3ab')
var roles = [
  acrPullRole
  sbDataSenderRole
  sbDataReceiverRole
  storageAccountStorageBlobDataContributorRole
  storageAccountContributorRole
]

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: 'test-master-services-access-identity'
  location: resourceGroup().location
}

@batchSize(3)
resource uaiRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for role in roles: {
  name: guid(resourceGroup().id, uai.id, role)
  properties: {
    roleDefinitionId: role
    principalId: uai.properties.principalId
    principalType: 'ServicePrincipal'
  }
}]
