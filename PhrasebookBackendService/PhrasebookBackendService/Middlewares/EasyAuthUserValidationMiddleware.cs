using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Phrasebook.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Middlewares
{
    public class EasyAuthUserValidationMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<EasyAuthUserValidationMiddleware> logger;

        public EasyAuthUserValidationMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            this.logger = loggerFactory?.CreateLogger<EasyAuthUserValidationMiddleware>() ?? throw new ArgumentNullException(nameof(loggerFactory));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string principalId, identityProvider;
            IEnumerable<Claim> claims;

            if (context.Request.Headers.ContainsKey(Constants.EasyAuthPrincipalIdHeaderName))
            {
                this.logger.LogInformation("EasyAuth headers from Azure App Service authentication middleware were found. Setting current HTTP context user to the authenticated user.");

                // Special headers automatically injected by Azure App Service
                principalId = context.Request.Headers[Constants.EasyAuthPrincipalIdHeaderName].FirstOrDefault();
                identityProvider = context.Request.Headers[Constants.EasyAuthIdentityProviderHeaderName].FirstOrDefault();
                this.logger.LogInformation($"Principal ID: {principalId}{Environment.NewLine}Identity Provider: {identityProvider}");

                // Decode the Client Principal from the request header
                string clientPrincipalEncoded = context.Request.Headers[Constants.EasyAuthEncodedPrincipalHeaderName].FirstOrDefault();
                byte[] decodedBytes = Convert.FromBase64String(clientPrincipalEncoded);
                string clientPrincipalDecoded = System.Text.Encoding.Default.GetString(decodedBytes);

                EasyAuthClientPrincipal clientPrincipal = JsonConvert.DeserializeObject<EasyAuthClientPrincipal>(clientPrincipalDecoded);
                claims = clientPrincipal.Claims.Select(x => new Claim(x.Type, x.Value));
            }
            else
            {
                // Development mode: set test user
                principalId = Constants.DevelopmentUserPrincipalId;
                identityProvider = Constants.DevelopmentUserIdentityProvider;
                claims = new Claim[]
                {
                    new Claim(Constants.EasyAuthFullNameClaimType, Constants.DevelopmentUserFullName),
                    new Claim(Constants.EasyAuthEmailClaimType, Constants.DevelopmentUserEmail),
                };
            }

            context.User = this.GetPrincipal(principalId, identityProvider, claims);

            await this.next(context);
        }

        private ClaimsPrincipal GetPrincipal(string principalId, string identityProvider, IEnumerable<Claim> claims)
        {
            GenericIdentity identity = new GenericIdentity(principalId, identityProvider);
            identity.AddClaims(claims);
            return new GenericPrincipal(identity, null);
        }

        public class EasyAuthClientPrincipal
        {
            [JsonProperty("auth_typ")]
            public string AuthenticationType { get; set; }

            [JsonProperty("claims")]
            public IEnumerable<EasyAuthUserClaim> Claims { get; set; }

            [JsonProperty("name_typ")]
            public string NameType { get; set; }

            [JsonProperty("role_typ")]
            public string RoleType { get; set; }
        }

        public class EasyAuthUserClaim
        {
            [JsonProperty("typ")]
            public string Type { get; set; }

            [JsonProperty("val")]
            public string Value { get; set; }
        }
    }
}
