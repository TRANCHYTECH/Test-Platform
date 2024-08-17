param location string = resourceGroup().location
param containerAppEnvName string = 'tranchy-cae-testplatf-dev-sa'

var subDomains = [
  'testportal-api'
  'testrunner-api'
]

resource containerAppEnv 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: containerAppEnvName
}

resource managedEnvironmentManagedCertificate 'Microsoft.App/managedEnvironments/managedCertificates@2024-03-01' = [
  for subDomain in subDomains: {
    parent: containerAppEnv
    name: '${containerAppEnv.name}-${subDomain}-certificate'
    location: location
    properties: {
      subjectName: '${subDomain}-dev.tranchy.tech'
      domainControlValidation: 'TXT'
    }
  }
]
