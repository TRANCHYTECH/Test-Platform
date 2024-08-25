param sqlServerName string
param sqlDbName string
param sqlAdmin string
param sqlAdminPass string
param location string = resourceGroup().location
param userManagedIdentity string

resource uai 'Microsoft.ManagedIdentity/userAssignedIdentities@2023-07-31-preview' existing = {
  name: userManagedIdentity
}

resource sqlServer 'Microsoft.Sql/servers@2023-08-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    publicNetworkAccess: 'Enabled'
    restrictOutboundNetworkAccess: 'Disabled'
    administratorLogin: sqlAdmin
    administratorLoginPassword: sqlAdminPass
  }
}

resource sqlServerDatabase 'Microsoft.Sql/servers/databases@2023-08-01-preview' = {
  parent: sqlServer
  name: sqlDbName
  location: location
  sku: {
    name: 'Basic'
    tier: 'Basic'
    capacity: 5
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 2147483648
    catalogCollation: 'SQL_Latin1_General_CP1_CI_AS'
  }
}

output sqlConnectionString string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName};Authentication=Active Directory Managed Identity;User Id=${uai.properties.principalId};Database=${sqlServerDatabase.name};'
output sqlConnectionString2 string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${sqlServerDatabase.name};Persist Security Info=False;User ID=${sqlAdmin};Password=${sqlAdminPass};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
