using './main.bicep'

param location = ''
param containerAppLogAnalyticsName = 'log-vg-tm-dev-sa-001'
param containerAppEnvName = 'cae-vg-tm-dev-sa-001'
param targetPort = 80
param containerImage = 'mcr.microsoft.com/k8se/quickstart:latest'
param cpuCore = '0.25'
param memorySize = '0.5'
param minReplicas = 1
param maxReplicas = 3

