using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Phrasebook.Common;
using Phrasebook.Data;
using Phrasebook.Data.Validation;
using PhrasebookBackendService.Authorization;

namespace PhrasebookBackendService.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ITimeProvider, TimeProvider>();

            // Inject the DB context so the controllers can get the instance via Dependency Injection
            services.AddDbContext<PhrasebookDbContext>(options =>
            {
                // We need to specify the MigrationsAssembly because it has to match the assembly that the derived DbContext belongs to
                // This is needed to be able to find the correct migration files to apply at runtime
                options.SetSqlConnectionString(this.Configuration, x => x.MigrationsAssembly(Constants.MigrationsAssembly));
            });

            services.AddScoped<IValidatorFactory, ValidatorFactory>();
            services.AddScoped<IAuthorizationHandler, SignupHandler>();

            services.AddAuthentication(options =>
            {
                options.DefaultForbidScheme = Constants.AuthenticationSchemeName;
                options.AddScheme<SignupAuthenticationHandler>(Constants.AuthenticationSchemeName, Constants.AuthenticationSchemeName);
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.UserIsSignedUpPolicy, policy => policy.AddRequirements(new SignupRequirement()));
            });

            services.AddControllers()
                .AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // TODO: set exception handling middleware
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            // Any custom middleware must be setup before UseEndpoints()
            app.UseEasyAuthUserValidation();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
