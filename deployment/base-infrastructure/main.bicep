param userAssignedIdentityBackOffice string
param containerRegistryName string
param serviceBusNamespace string
param keyVaultName string
param sqlServerName string
param sqlDbName string
param storageAccountName string
param logWorkspaceName string
param containerAppEnvName string

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
    accountName: storageAccountName
    userAssignedIdentity: userAssignedIdentityBackOffice
    location: resourceGroup().location
  }
  dependsOn: [
    uaiBackOffice
  ]
}

module containerRegistry 'container-registry.bicep' = {
  name: 'containerRegistryDeploy'
  params: {
    containerRegistryName: containerRegistryName
  }
}

module logWorkspace 'log-workspace.bicep' = {
  name: 'logWorkspaceDeploy'
  params: {
    logWorkspaceName: logWorkspaceName
    location: resourceGroup().location
  }
}
module sb 'service-bus.bicep' = {
  name: 'serviceBusDeploy'
  params: {
    serviceBusNamespace: serviceBusNamespace
  }
}

module kv 'key-vault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    keyVaultName: keyVaultName
  }
}

module sql 'sql.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
    location: resourceGroup().location
  }
}

module apps 'container-apps.bicep' = {
  name: 'appsDeploy'
  params: {
    containerAppEnvName: containerAppEnvName
    logWorkspaceName: logWorkspaceName
  }
}

output uaiIdBackOffice string = uaiBackOffice.outputs.uaiId
