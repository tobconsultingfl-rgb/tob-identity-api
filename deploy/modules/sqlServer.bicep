param sqlServerName string
param location string
param tags object

var sqlServerAdministratorLogin = 'sqladmin'
var sqlServerAdministratorLoginPassword = 'Admin@1234' //'J3#pI7P&Qz!206tz)0'

resource sqlServer 'Microsoft.Sql/servers@2021-05-01-preview' = {
  name: sqlServerName
  location: location
  tags: tags
  properties: {
    administratorLogin: sqlServerAdministratorLogin
    administratorLoginPassword: sqlServerAdministratorLoginPassword
  }
}

resource sqlServerFirewallRule 'Microsoft.Sql/servers/firewallRules@2021-05-01-preview' = {
  parent: sqlServer
  name: 'AllowAllWindowsAzureIps'
  properties: {
    endIpAddress: '0.0.0.0'
    startIpAddress: '0.0.0.0'
  }
}

output sqlServer object = sqlServer
output sqlServerFirewallRule object = sqlServerFirewallRule
output sqlUsername string = sqlServerAdministratorLogin
output sqlPassword string = sqlServerAdministratorLoginPassword
