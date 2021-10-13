using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;

namespace Phrasebook.Data
{
    /// <summary>
    /// The code in this class uses the Azure.Identity library so that it can authenticate and retrieve an access token for the database,
    /// no matter where the code is running.
    /// If you're running on your local machine, DefaultAzureCredential() loops through a number of options to find a valid account that is logged in.
    /// More about DefaultAzureCredential class: https://docs.microsoft.com/en-us/dotnet/api/azure.identity.defaultazurecredential
    /// </summary>
    public class CustomAzureSqlAuthProvider : SqlAuthenticationProvider
    {
        private static readonly string[] azureSqlScopes = new[]
        {
            "https://database.windows.net//.default"
        };
        private static readonly TokenCredential credential = new DefaultAzureCredential();

        public override async Task<SqlAuthenticationToken> AcquireTokenAsync(SqlAuthenticationParameters parameters)
        {
            var tokenRequestContext = new TokenRequestContext(azureSqlScopes);
            var tokenResult = await credential.GetTokenAsync(tokenRequestContext, default);
            return new SqlAuthenticationToken(tokenResult.Token, tokenResult.ExpiresOn);
        }

        public override bool IsSupported(SqlAuthenticationMethod authenticationMethod) => authenticationMethod.Equals(SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow);
    }
}
