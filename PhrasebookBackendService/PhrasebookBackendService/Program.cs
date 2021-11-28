using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phrasebook.Common.Constants;
using Phrasebook.Data.Development;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Web;

namespace PhrasebookBackendService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            if (Environment.GetEnvironmentVariable(Constants.AspNetCoreEnvironmentVariableName) == Constants.Development)
            {
                // Only in DEV mode: we create the DB if it doesn't exist + seed test data + apply any pending migration
                // TODO: In PROD, we're expected to do this manually via dotnet CLI, until we move to SQL scripts
                CreateDbIfNotExists(host);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    // Support Azure App Services 'Diagnostics logs' and 'Log stream' features.
                    logging.AddAzureWebAppDiagnostics();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void CreateDbIfNotExists(IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var unitOfWork = services.GetRequiredService<IUnitOfWork>();
                DbInitializer.InitializeAsync(unitOfWork).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, $"An error occurred creating the DB: {ex.Message}");
                throw;
            }
        }
    }
}
