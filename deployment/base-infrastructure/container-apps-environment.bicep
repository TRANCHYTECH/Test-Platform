param containerAppEnvName string
param location string = resourceGroup().location
param logWorkspaceName string
param mongoStateMetadata object[]
param mongoStateScopes string[]

resource logWorkspace 'Microsoft.OperationalInsights/workspaces@2023-09-01' existing = {
  name: logWorkspaceName
}

resource managedEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: containerAppEnvName
  location: location
  properties: {
    daprConfiguration: {}
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logWorkspace.properties.customerId
        sharedKey: logWorkspace.listKeys().primarySharedKey
      }
    }
    workloadProfiles: [
      {
        name: 'Consumption'
        workloadProfileType: 'Consumption'
      }
    ]
  }
  dependsOn: [logWorkspace]
}

resource mongodbState 'Microsoft.App/managedEnvironments/daprComponents@2024-03-01' = {
  name: 'test-runner-state'
  parent: managedEnvironment
  properties: {
    componentType: 'state.mongodb'
    version: 'v1'
    metadata: mongoStateMetadata
    scopes: mongoStateScopes
  }
}
