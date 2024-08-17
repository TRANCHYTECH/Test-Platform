param keyValueName string
param secretPairs array

resource keyVault 'Microsoft.KeyVault/vaults@2024-04-01-preview' existing = {
  name: keyValueName
}

resource secret 'Microsoft.KeyVault/vaults/secrets@2024-04-01-preview' = [
  for pair in secretPairs: {
    parent: keyVault
    name: pair.name
    properties: {
      value: pair.value
    }
  }
]
