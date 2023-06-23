using './container-app-dapr.bicep'

param env = [
  {
    name: 'ASPNETCORE_ENVIRONMENT'
    value: 'Development'
  }
]

param location = ''
param containerAppName = ''
param environmentName = ''
param containerRegistry = ''
param subDomainCertificate = ''
