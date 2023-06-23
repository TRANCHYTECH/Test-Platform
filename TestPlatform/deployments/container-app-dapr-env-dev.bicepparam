using './container-app-dapr.bicep'

param env = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Development'
  }
]

param location = ''
param containerAppName = ''
param targetPort = 80
param containerImage = 'mcr.microsoft.com/k8se/quickstart:latest'
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3
param environmentName = ''
param revisionMode = 'Single'
param containerRegistry = ''
param subDomainCertificate = ''
