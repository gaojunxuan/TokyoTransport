using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TokyoTransport.Helper
{
    public class TokenHelper
    {
        public static async Task<string> GetToken(string name)
        {
            // Pull the connection string out of Azure Key Vault
            string sourceStorageConnectionStringKvUrl = $"https://slwsp-keyvault.vault.azure.net/secrets/{name}";
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var connectionStringSecret = await keyVaultClient.GetSecretAsync(sourceStorageConnectionStringKvUrl).ConfigureAwait(false);
            string connectionString = connectionStringSecret.Value;
            return connectionString;
        }
    }
}
