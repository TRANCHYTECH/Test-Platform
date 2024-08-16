using './backoffice-container-app.bicep'

param location = ''
param containerAppName = ''
param targetPort = 8080
param containerImage = ''
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3
param environmentName = ''
param revisionMode = 'Single'
param containerRegistry = ''
param subDomainCertificate = ''
param userAssignedIdentity =  ''
