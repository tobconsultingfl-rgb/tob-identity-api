param appServicePlanName string
param environmentType string
param location string
param tags object

// Define the SKUs for each component based on the environment type.
var environmentConfigurationMap = {
  Feature: {
    appServicePlan: {
      sku: {
        capacity: 1
        name: 'S1'
      }
    }
  }   
  Develop: {
    appServicePlan: {
      sku: {
        name: 'S1'
      }
    }
  }  
  Test: {
    appServicePlan: {
      sku: {
        name: 'S1'
        capacity: 1
      }
    }
  }
  Prod: {
    appServicePlan: {
      sku: {
        name: 'S1'
        capacity: 1
      }
    }
  }  
}

resource appServicePlan 'Microsoft.Web/serverfarms@2021-03-01' = {
  name: appServicePlanName
  location: location
  kind: 'windows'   
  sku: environmentConfigurationMap[environmentType].appServicePlan.sku
  tags: tags
}

output serverFarmId string = appServicePlan.id
