param keyVaultName string
param secretName string
param secretValue string

resource existingKeyVault 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: keyVaultName  
}

resource keyVaultSecret 'Microsoft.KeyVault/vaults/secrets@2021-11-01-preview' = {
  parent: existingKeyVault
  name: secretName
  properties: {
    value: secretValue
  }
}
