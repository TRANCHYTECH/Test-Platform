param userAssignedIdentity string
param location string = resourceGroup().location

// Roles
var roles = {
  acrPullRole: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  sbDataSenderRole:  '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
  sbDataReceiverRole: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
  storageAccountStorageBlobDataContributorRole: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  storageAccountContributorRole: '17d1049b-9a84-46fb-8f53-869881c3d3ab'
}

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' = {
  name: userAssignedIdentity
  location: location
}

@batchSize(3)
resource uaiRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = [for role in items(roles): {
  name: guid(resourceGroup().id, uai.id, role.key)
  properties: {
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', role.value)
    principalId: uai.properties.principalId
    principalType: 'ServicePrincipal'
  }
}]


output uaiId string = uai.id
