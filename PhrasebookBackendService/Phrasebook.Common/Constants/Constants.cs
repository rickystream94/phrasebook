using System;
using System.IO;

namespace Phrasebook.Common.Constants
{
    public static partial class Constants
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

        #region Phrases

        public const int MaxPhraseLength = 100;

        public const int MaxSynonymsLength = 500;

        #endregion

        #region Local Development

        public const string DevelopmentUserPrincipalId = "7c5c128c-395b-4610-874b-cddc0ed1a8ef";

        public const string DevelopmentUserEmail = "dev@test.com";

        public const string DevelopmentUserFullName = "Dev User";

        public const string DevelopmentUserIdentityProvider = "dev";

        #endregion

        public static readonly string SupportedLanguagesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Languages", "supportedLanguages.json");

        private const string AppSettingsJsonFormat = "appsettings.{0}.json";

        public static string GetConfigurationFile(string environment) => string.Format(AppSettingsJsonFormat, environment);
    }
}
