param location string = resourceGroup().location
param serviceBusNamespace string

resource sbNamespace 'Microsoft.ServiceBus/namespaces@2022-10-01-preview' = {
  name: serviceBusNamespace
  location: location
  sku: {
    name: 'Standard'
    capacity: 1
    tier: 'Standard'
  }
}
