using './container-app-dapr.bicep'

param location = 'southeastasia'
param environmentName = 'cae-vg-tm-dev-sa-001'

param containerAppName = ''
param targetPort = 80
param containerImage = ''
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3
param revisionMode = 'Single'
param containerRegistry = ''
param subDomainCertificate = ''
