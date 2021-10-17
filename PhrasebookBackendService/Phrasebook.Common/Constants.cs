namespace Phrasebook.Common
{
    public static class Constants
    {
        public const string AspNetCoreEnvironmentVariableName = "ASPNETCORE_ENVIRONMENT";

        public const string Production = "Production";

        public const string Development = "Development";

        public const string DatabaseConnectionStringName = "Database";

        public const string MigrationsAssembly = "PhrasebookBackendService";

        #region EasyAuth

        public const string EasyAuthPrincipalNameHeaderName = "X-MS-CLIENT-PRINCIPAL-NAME";

        public const string EasyAuthPrincipalIdHeaderName = "X-MS-CLIENT-PRINCIPAL-ID";

        public const string EasyAuthIdentityProviderHeaderName = "X-MS-CLIENT-PRINCIPAL-IDP";

        public const string EasyAuthEncodedPrincipalHeaderName = "X-MS-CLIENT-PRINCIPAL";

        public const string EasyAuthFullNameClaimType = "name";

        public const string EasyAuthEmailClaimType = "preferred_username";

        #endregion

        #region Authorization

        public const string UserIsSignedUpPolicy = "UserIsSignedUp";

        public const string AuthenticationSchemeName = "Signup";

        #endregion

        private const string AppSettingsJsonFormat = "appsettings.{0}.json";

        public static string GetConfigurationFile(string environment) => string.Format(AppSettingsJsonFormat, environment);
    }
}
