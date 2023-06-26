@description('Specifies the location for resource.')
param location string

@description('Specifies the name of the container app.')
param containerAppName string

param aspNetEnv string = 'Development'

@description('Specifies the container port.')
param targetPort int = 80

@description('Specifies the docker container image to deploy.')
param containerImage string = 'mcr.microsoft.com/k8se/quickstart:latest'

@description('Number of CPU cores the container can use. Can be with a maximum of two decimals.')
@allowed([
  '0.25'
  '0.5'
  '0.75'
  '1'
  '1.25'
  '1.5'
  '1.75'
  '2'
])
param cpuCore string = '0.25'

@description('Amount of memory (in gibibytes, GiB) allocated to the container up to 4GiB. Can be with a maximum of two decimals. Ratio with CPU cores must be equal to 2.')
@allowed([
  '0.5'
  '1'
  '1.5'
  '2'
  '3'
  '3.5'
  '4'
])
param memorySize string = '0.5'

@description('Minimum number of replicas that will be deployed')
@minValue(0)
@maxValue(25)
param minReplicas int = 1

@description('Maximum number of replicas that will be deployed')
@minValue(0)
@maxValue(25)
param maxReplicas int = 3

param environmentName string
param revisionMode string = 'Single'
param containerRegistry string
param subDomainCertificate string

resource environment 'Microsoft.App/managedEnvironments@2023-04-01-preview' existing = {
  name: environmentName
}

resource managedEnvironmentManagedCertificate 'Microsoft.App/managedEnvironments/managedCertificates@2023-04-01-preview' existing = {
  name: subDomainCertificate
  parent: environment
}

var serviceBusNameSpaceName = 'vgtrunnerdev'

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' existing = {
  name: serviceBusNameSpaceName
}
resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' existing = {
  name: 'test-master-services-access-identity'
}

resource containerApp 'Microsoft.App/containerApps@2022-06-01-preview' = {
  name: containerAppName
  location: location
  identity: {
    type: 'UserAssigned'
    userAssignedIdentities: {
      '${uai.id}': {}
    }
  }
  properties: {
    environmentId: environment.id
    configuration: {
      activeRevisionsMode: revisionMode
      registries: [
        {
          identity: uai.id
          server: containerRegistry
        }
      ]
      dapr: {
        enabled: true
        appId: containerAppName
        appPort: 80
        appProtocol: 'http'
        enableApiLogging: true
      }
      ingress: {
        external: true
        targetPort: targetPort
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
        customDomains: [
          {
            name: managedEnvironmentManagedCertificate.properties.subjectName
            certificateId: managedEnvironmentManagedCertificate.id
            bindingType: 'SniEnabled'
          }
        ]
      }
    }
    template: {
      containers: [
        {
          name: containerAppName
          image: containerImage
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: aspNetEnv
            }
            {
              name: 'TestManagerServiceBus__Namespace'
              value: serviceBusNamespace.name
            }
            {
              name: 'TestManagerServiceBus__ManagedIdentityClientId'
              value: uai.properties.clientId
            }
          ]
          resources: {
            cpu: json(cpuCore)
            memory: '${memorySize}Gi'
          }
        }
      ]
      scale: {
        minReplicas: minReplicas
        maxReplicas: maxReplicas
      }
    }
  }
}

output fqdn string = containerApp.properties.configuration.ingress.fqdn
