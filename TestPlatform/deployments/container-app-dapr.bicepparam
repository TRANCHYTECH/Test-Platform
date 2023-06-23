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
param containerImage = 'vgeektestmasterdev.azurecr.io/vietgeekstestplatformtestmanagerapi:725769e3245887652b0adf7447b5eb415e35ba68'
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3
param environmentName = 'cae-vg-tm-dev-sa-001'
param revisionMode = 'Single'
param containerRegistry = 'vgeektestmasterdev.azurecr.io'
param subDomainCertificate = 'dev-test-manager-api'
