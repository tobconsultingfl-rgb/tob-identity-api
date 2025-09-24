param appServiceName string
param serverFarmId string
param location string
param tags object
param appSettings array

resource appService 'Microsoft.Web/sites@2021-03-01' = {
  name: appServiceName
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  
  properties:{
    serverFarmId: serverFarmId
    siteConfig:{
      alwaysOn: false
      ftpsState: 'Disabled'
      netFrameworkVersion: 'v9.0'
    }
    httpsOnly: true
  }

  tags:tags  
}

resource appServiceConfiguration 'Microsoft.Web/sites/config@2021-03-01' = {
  parent: appService
  name: 'web'
  properties: {
    appSettings: appSettings   

    virtualApplications: [
      {
        physicalPath: 'site\\wwwroot'
        preloadEnabled: false
        virtualPath: '/'
      }
    ]
  }  
}

output appServicePrincipalId string = appService.identity.principalId
