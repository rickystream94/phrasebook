using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Phrasebook.Common;
using Phrasebook.Data;

namespace PhrasebookBackendService
{
    /// <summary>
    /// Factory class needed by dotnet CLI to run migrations
    /// </summary>
    public class PhrasebookDesignTimeDbContextFactory : IDesignTimeDbContextFactory<PhrasebookDbContext>
    {
        public PhrasebookDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile(Constants.AppSettingsJson)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PhrasebookDbContext>();
            optionsBuilder.SetSqlConnectionString(configuration, x => x.MigrationsAssembly(Constants.MigrationsAssembly));

            return new PhrasebookDbContext(optionsBuilder.Options);
        }
    }
}
