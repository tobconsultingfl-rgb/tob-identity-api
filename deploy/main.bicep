@description('The location into which your Azure resources should be deployed.')
param location string = resourceGroup().location

@minLength(2)
@description('Azure Devops Project Name')
param applicationName string = 'UndefinedapplicationName'

@description('Key Vault Containing Secrets.')
param keyVaultName string = ''

@description('Azure AD Instance.')
@minLength(3)
param azureADInstance string
@description('Azure AD Domain.')
@minLength(3)
param azureADDomain string
@description('Azure AD Tenant ID.')
@minLength(3)
param azureADTenantId string
@description('Azure AD Client ID.')
@minLength(3)
param azureADClientId string
@description('Azure BAD2C Client Secret.')
@minLength(3)
param azureADClientSecret string


@description('Select the type of environment you want to provision. Case Sensitive!')
@allowed([
  'feature'
  'develop'
  'test'
  'prod'
])
param environmentType string

@description('Key Vault Containing Secrets.')
param keyVaultName string

// Define the names for resources.
var sqlServerName = 'sqldb-tobconsulting-${environmentType}-${location}'
var sqlDatabaseName = 'IdentityDb'
var appServicePlanName = 'plan-${applicationName}-${environmentType}-${location}'
var appServiceName = 'as-${applicationName}-${environmentType}-${location}'
var applicationInsightsName = 'appi-${applicationName}-${environmentType}-${location}'

var tags = {
  'Owner': 'TOB Consulting'
  'Env': environmentType
  'DR': 'Low'
  'Project': 'TOB Identity API'
}

var appConfigSettings = [

]

module sqlServer './modules/sqlServer.bicep' = {
  name: 'sqlServiceDeploy'
  params: {
    sqlServerName: sqlServerName
    location: location
    tags: tags
  }
}

module identityDatabase './modules/sqlDatabase.bicep' = {
  dependsOn: [
    sqlServer
  ]
  name: 'identityDbDeploy'
  params: {
    sqlServerName: sqlServerName
    sqlUsername: sqlServer.outputs.sqlUsername
    sqlPassword: sqlServer.outputs.sqlPassword
    databaseName: sqlDatabaseName
    environmentType: environmentType
    location: location
    tags: tags
  }
}
 
module appServicePlan './modules/appServicePlan.bicep' = {
  name: 'appServicePlanDeploy'
  params: {
    appServicePlanName: appServicePlanName    
    location: location
    environmentType: environmentType
    tags: tags
  }
}

module appService './modules/appService.bicep' = {
  name: 'appServiceDeploy'
  params: {
    appServiceName: appServiceName
    serverFarmId: appServicePlan.outputs.serverFarmId
    location: location
    appSettings: appConfigSettings
    tags: tags
  }
}

module identityConnectionStringSecret './modules/keyVaultSecret.bicep' = {
  dependsOn: [
    bohaCoreDatabase
  ]
  name: 'identityDatabaseConnectionStringSecretDeploy'
  params: {
    keyVaultName: keyVaultName
    secretName: 'ConnectionStrings--IdentityDBContext'
    secretValue: identityDatabase.outputs.connectionString
  }
}


module azureADInstanceSecret './modules/keyVaultSecret.bicep' = {
  name: 'azureADInstanceSecret'
  params: {
    keyVaultName: keyVaultName
    secretName: 'AzureAD--Instance'
    secretValue: azureADInstance
  }
}

module azureADDomainSecret './modules/keyVaultSecret.bicep' = {
  name: 'azureADDomainSecret'
  params: {
    keyVaultName: keyVaultName
    secretName: 'AzureAD--Domain'
    secretValue: azureADDomain
  }
}
 
module azureADTenantIdSecret './modules/keyVaultSecret.bicep' = {
  name: 'azureADTenantIdSecret'
  params: {
    keyVaultName: keyVaultName
    secretName: 'AzureAD--TenantId'
    secretValue: azureADTenantId
  }
}
 
module azureADClientIdSecret './modules/keyVaultSecret.bicep' = {
  name: 'azureADClientIdSecret'
  params: {
    keyVaultName: keyVaultName
    secretName: 'AzureAD--ClientId'
    secretValue: azureB2cClientId
  }
}
 
module azureADClientSecretSecret './modules/keyVaultSecret.bicep' = {
  name: 'azureADClientSecretSecret'
  params: {
    keyVaultName: keyVaultName
    secretName: 'AzureAD--B2CClientSecret'
    secretValue: azureADClientSecret
  }
}

