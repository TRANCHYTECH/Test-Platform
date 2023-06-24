@description('Specifies the location for all resources.')
param location string = resourceGroup().location

@description('Specifies the name of the log analytics workspace.')
param containerAppLogAnalyticsName string = 'log-vg-tm-dev-sa-001'

@description('Specifies the name of the container app environment.')
param containerAppEnvName string = 'cae-vg-tm-dev-sa-001'

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

var containerAppNames = [
  'ca-vg-tm-tmgrapp-dev-sa-001'
  'ca-vg-tm-tmgrapi-dev-sa-001'
  'ca-vg-tm-trunnerapp-dev-sa-001'
  'ca-vg-tm-trunnerapi-dev-sa-001'
  'ca-vg-tm-amgrapi-dev-sa-001'
]

var serviceBusConnectionStringName = 'sb-root-connectionstring'

resource logAnalytics 'Microsoft.OperationalInsights/workspaces@2022-10-01' = {
  name: containerAppLogAnalyticsName
  location: location
  properties: {
    sku: {
      name: 'PerGB2018'
    }
  }
}

var serviceBusNameSpaceName = 'vgtrunnerdev'

var queueNames = [
  'user-create-request'
  'access-code-email-notification'
  'send-test-access-code'
]

resource serviceBusNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNameSpaceName
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    disableLocalAuth: false
  }

  resource serviceBusQueue 'queues' = [for queueName in queueNames: {
    name: queueName
    properties: {
      lockDuration: 'PT5M'
      maxSizeInMegabytes: 1024
      requiresDuplicateDetection: false
      requiresSession: false
      defaultMessageTimeToLive: 'P10675199DT2H48M5.4775807S'
      deadLetteringOnMessageExpiration: false
      duplicateDetectionHistoryTimeWindow: 'PT10M'
      maxDeliveryCount: 10
      autoDeleteOnIdle: 'P10675199DT2H48M5.4775807S'
      enablePartitioning: false
      enableExpress: false
    }
  }]
}

var acrPullRole = resourceId('Microsoft.Authorization/roleDefinitions', '7f951dda-4ed3-4680-a7ca-43fe172d538d')

// Create user assigned identity so that could use to pull image
resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2022-01-31-preview' = {
  name: 'id-ca-vg-tm-tmgrapi-dev-sa-001'
  location: location
}

resource uaiRbac 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(resourceGroup().id, uai.id, acrPullRole)
  properties: {
    roleDefinitionId: acrPullRole
    principalId: uai.properties.principalId
    principalType: 'ServicePrincipal'
  }
}

resource sbAuthRule 'Microsoft.ServiceBus/namespaces/AuthorizationRules@2021-11-01' existing = {
  name: 'RootManageSharedAccessKey'
  parent: serviceBusNamespace
}

resource containerAppEnv 'Microsoft.App/managedEnvironments@2023-04-01-preview' = {
  name: containerAppEnvName
  location: location
  properties: {
    daprConfiguration: {}
    appLogsConfiguration: {
      destination: 'log-analytics'
      logAnalyticsConfiguration: {
        customerId: logAnalytics.properties.customerId
        sharedKey: logAnalytics.listKeys().primarySharedKey
      }
    }
  }
}

resource daprServiceBus 'Microsoft.App/managedEnvironments/daprComponents@2023-04-01-preview' = {
  name: 'webhook-pubsub'
  parent: containerAppEnv
  properties: {
    componentType: 'pubsub.azure.servicebus.queues'
    version: 'v1'
    secrets: [
      {
        name: serviceBusConnectionStringName
        value: sbAuthRule.listKeys().primaryConnectionString
      }
    ]
    metadata: [
      {
        name: 'connectionString'
        secretRef: serviceBusConnectionStringName
      }
    ]
    scopes: containerAppNames
  }
}

resource daprStateMongoDb 'Microsoft.App/managedEnvironments/daprComponents@2023-04-01-preview' = {
  name: 'general-notify-store'
  parent: containerAppEnv
  properties: {
    componentType: 'state.mongodb'
    version: 'v1'
    scopes: containerAppNames
    metadata: [
      {
        name: 'server'
        value: 'cluster0.agnomfq.mongodb.net'
      }
      {
        name: 'username'
        value: 'testmaster'
      }
      {
        name: 'password'
        value: 'jZeAEmK8TwGslaiP'
      }
      {
        name: 'databaseName'
        value: 'TestRunnerDev'
      }
      {
        name: 'collectionName'
        value: 'testnotification'
      }
      {
        name: 'operationTimeout'
        value: '10s'
      }
      {
        name: 'writeConcern'
        value: 'majority'
      }
      {
        name: 'actorStateStore'
        value: 'true'
      }
    ]
  }
}

resource storageAccount 'Microsoft.Storage/storageAccounts@2022-09-01' = {
  name: 'vgtmgrdev'
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {}
}

var containerRegistryName = 'vgeektestmasterdev'
module acr 'containerRegistry.bicep' = {
  name: 'acr'
  params: {
    location: location
    containerRegistryName: containerRegistryName
  }
}

var subDomains  = [
  'test-manager-api'
  'test-manager'
  'test-runner-api'
  'test-runner'
  'webhook'
]

resource customDomainsProvisioningContainerApp 'Microsoft.App/containerApps@2022-06-01-preview' = {
  name: 'vgeek-custom-domains'
  location: location
  properties: {
    environmentId: containerAppEnv.id
    configuration: {
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
        customDomains: [for item in subDomains: {
          name: 'dev.${item}.testmaster.io'
          bindingType: 'Disabled'
        }]
      }
    }
    template: {
      revisionSuffix: 'firstrevision'
      containers: [
        {
          name: 'vgeek-customdomains-provisioning'
          image: 'mcr.microsoft.com/k8se/quickstart:latest'
          resources: {
            cpu: json(cpuCore)
            memory: '${memorySize}Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 2
      }
    }
  }
}

@batchSize(5)
resource managedEnvironmentManagedCertificate 'Microsoft.App/managedEnvironments/managedCertificates@2023-04-01-preview' = [for item in subDomains: {
  parent: containerAppEnv
  name: 'dev-${item}-certificate'
  location: location
  properties: {
    subjectName: 'dev.${item}.testmaster.io'
    domainControlValidation: 'TXT'
  }
  dependsOn: [
    customDomainsProvisioningContainerApp
  ]
}]

// @batchSize(5)
// module containerApps 'containerApp.bicep' = [for appName in containerAppNames: {
//   name: '${deployment().name}-${appName}'
//   params: {
//     location: location
//     containerAppEnvId: containerAppEnv.id
//     containerAppName: appName
//     minReplicas: minReplicas
//     maxReplicas: maxReplicas
//     cpuCore: cpuCore
//     memorySize: memorySize
//     containerImage: containerImage
//     targetPort: targetPort
//   }
// }]
