using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Phrasebook.Common;
using Phrasebook.Data;
using System;

namespace PhrasebookBackendService
{
    /// <summary>
    /// Factory class needed by dotnet CLI to run migrations
    /// </summary>
    public class PhrasebookDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PhrasebookDbContext>
    {
        public PhrasebookDbContext CreateDbContext(string[] args)
        {
            string environment = Environment.GetEnvironmentVariable(Constants.AspNetCoreEnvironmentVariableName);
            if (string.IsNullOrWhiteSpace(environment))
            {
                throw new ArgumentNullException(nameof(environment));
            }

            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Constants.GetConfigurationFile(environment))
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PhrasebookDbContext>();
            optionsBuilder.SetSqlConnectionString(configuration, x => x.MigrationsAssembly(Constants.MigrationsAssembly));

            return new PhrasebookDbContext(optionsBuilder.Options);
        }
    }
}
