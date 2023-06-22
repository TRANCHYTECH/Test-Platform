using './container-app-dapr.bicep'

param location = 'southeastasia'
param containerAppName = 'ca-vg-tm-tmgrapi-dev-sa-001'
param env = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Development'
  }
]
param targetPort = 80
param containerImage = 'mcr.microsoft.com/k8se/quickstart:latest'
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3
param environmentName = 'cae-vg-tm-dev-sa-001'
param revisionMode = 'Single'
param containerRegistry = 'vgeektestmasterdev.azurecr.io'
param containerRegistryUsername = ''
param registryPassword = ''

