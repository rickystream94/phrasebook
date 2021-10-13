using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Phrasebook.Common;
using System;

namespace Phrasebook.Data
{
    public static class DbContextExtensions
    {
        public static void SetSqlConnectionString(
            this DbContextOptionsBuilder options,
            IConfiguration configuration,
            Action<SqlServerDbContextOptionsBuilder> sqlServerDbContextOptionsBuilderAction = null)
        {
            string environment = Environment.GetEnvironmentVariable(Constants.AspNetCoreEnvironmentVariableName);

            if (string.IsNullOrWhiteSpace(environment))
            {
                throw new ArgumentNullException(nameof(environment), $"'{Constants.AspNetCoreEnvironmentVariableName}' environment variable was not set.");
            }

            // We choose the proper connection string to use based on the environment name
            bool shouldUseProductionDb = string.Equals(environment, Constants.Production, StringComparison.OrdinalIgnoreCase);
            string connectionStringName = shouldUseProductionDb ? Constants.AzureAad : Constants.LocalDb;
            string connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), $"Couldn't find a value for the connection string.");
            }

            if (shouldUseProductionDb)
            {
                // Configure AAD auth to connect to Azure SQL DB
                // If running locally, use account that is setup in VS for Azure Auth (Tools > Options > Azure Service Authentication > Account Selection)
                // If running in Azure, a Managed Identity will be used
                SqlAuthenticationProvider.SetProvider(
                    SqlAuthenticationMethod.ActiveDirectoryDeviceCodeFlow,
                    new CustomAzureSqlAuthProvider());
                var sqlConnection = new SqlConnection(connectionString);
                options.UseSqlServer(sqlConnection, sqlServerDbContextOptionsBuilderAction);
            }
            else
            {
                // Dev mode: connect to local DB
                options.UseSqlServer(connectionString, sqlServerDbContextOptionsBuilderAction);
            }
        }
    }
}
