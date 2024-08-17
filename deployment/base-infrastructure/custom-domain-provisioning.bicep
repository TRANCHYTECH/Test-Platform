param location string = resourceGroup().location
param containerAppEnvName string = 'tranchy-cae-testplatf-dev-sa'
var subDomains = [
  'testportal-api'
  'testrunner-api'
]

resource containerAppEnv 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: containerAppEnvName
}

resource customDomainsProvisioningContainerApp 'Microsoft.App/containerApps@2023-05-01' = {
  name: 'testplatform-custom-domains'
  location: location
  properties: {
    environmentId: containerAppEnv.id
    configuration: {
      ingress: {
        external: true
        targetPort: 80
        allowInsecure: false
        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
        customDomains: [
          for item in subDomains: {
            name: '${item}-dev.tranchy.tech'
            bindingType: 'Disabled'
          }
        ]
      }
    }
    template: {
      revisionSuffix: 'firstrevision'
      containers: [
        {
          name: 'vgeek-customdomains-provisioning'
          image: 'mcr.microsoft.com/k8se/quickstart:latest'
          resources: {
            cpu: json('0.5')
            memory: '1Gi'
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
