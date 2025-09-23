param sqlServerName string
param location string
param tags object
param environmentType string
param databaseName string
param sqlUsername string
param sqlPassword string

var environmentConfigurationMap = {
  Feature: {
    sqlDatabase: {
      sku: {
        name: 'Basic'
        tier: 'Basic'
      }
    }
  }
  Develop: {
    sqlDatabase: {
      sku: {
        name: 'Basic'
        tier: 'Basic'
      }
    }
  }
  Test: {
    sqlDatabase: {
      sku: {
        name: 'Basic'
        tier: 'Basic'
      }
    }
  }
  Prod: {
    sqlDatabase: {
      sku: {
        name: 'Basic'
        tier: 'Basic'
      }
    }
  }  
}

resource sqlServer 'Microsoft.Sql/servers@2021-05-01-preview' existing = {
  name: sqlServerName
}

resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-02-01-preview' = {
  parent: sqlServer
  name: databaseName
  location: location
  tags: tags
  sku: environmentConfigurationMap[environmentType].sqlDatabase.sku
}

output connectionString string = 'Server=tcp:${sqlServer.name}.database.windows.net,1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${sqlUsername};Password=${sqlPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;'

