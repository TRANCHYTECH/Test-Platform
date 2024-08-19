param userAssignedIdentityBackOffice string
param containerRegistryName string
param serviceBusNamespace string
param keyVaultName string
param sqlServerName string
param sqlDbName string
param sqlAdmin string
@secure()
param sqlAdminPassword string
param storageAccountName string
param logWorkspaceName string
param containerAppEnvName string
param keyVaultSecretPairs array
param tenantObjectId string
param mongoStateMetadata object[]
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

module keyVault 'key-vault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    tenantId: tenant().tenantId
    tenantObjectId: tenantObjectId
    keyVaultName: keyVaultName
  }
}

module sql 'sql.bicep' = {
  name: 'sqlDeploy'
  params: {
    sqlServerName: sqlServerName
    sqlDbName: sqlDbName
    location: resourceGroup().location
    sqlAdmin: sqlAdmin
    sqlAdminPassword: sqlAdminPassword
  }
}

module apps 'container-apps-environment.bicep' = {
  name: 'appsEnvionmentDeploy'
  params: {
    containerAppEnvName: containerAppEnvName
    logWorkspaceName: logWorkspaceName
    mongoStateMetadata: mongoStateMetadata
  }
  dependsOn: [logWorkspace]
}

// Data
module keyVaultSecrets 'key-vault-secrets.bicep' = {
  name: 'keyVaultSecretsDeploy'
  params: {
    keyValueName: keyVaultName
    secretPairs: keyVaultSecretPairs
  }
  dependsOn: [keyVault]
}

output uaiIdBackOffice string = uaiBackOffice.outputs.uaiId
