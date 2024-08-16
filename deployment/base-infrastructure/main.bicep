// az deployment group create --resource-group Test --template-file .\main.bicep
param userAssignedIdentityBackOffice string = 'tranchy-uai-test-int-sa'
module uaiBackOffice 'user-assigned-identity.bicep' = {
  name: 'uaiDeploy'
  params: {
    userAssignedIdentity: userAssignedIdentityBackOffice
    location: resourceGroup().location
  }
}

module storageBackOffice 'storage.bicep' = {
  name: 'storageDeploy'
  params: {
    accountName: 'abcddfsgsgsgstestint'
    userAssignedIdentity: userAssignedIdentityBackOffice
    location: resourceGroup().location
  }
  dependsOn: [
    uaiBackOffice
  ]
}

output uaiIdBackOffice string = uaiBackOffice.outputs.uaiId
