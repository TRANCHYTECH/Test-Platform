using './container-app-dapr.bicep'

param location = 'southeastasia'
param containerAppName = 'ca-vg-tm-tmgrapp-dev-sa-001'
param env = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Development'
  }
]
param targetPort = 80
param containerImage = 'vgeektestmasterdev.azurecr.io/vietgeekstestplatformtestmanagerwebapp:15730c1d74f36be3b711c6cf7ab2367714b22832'
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3
param environmentName = 'cae-vg-tm-dev-sa-001'
param revisionMode = 'Single'
param containerRegistry = 'vgeektestmasterdev.azurecr.io'
param subDomainCertificate = 'dev-test-manager'
