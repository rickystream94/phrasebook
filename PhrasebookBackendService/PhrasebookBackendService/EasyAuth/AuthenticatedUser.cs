using Phrasebook.Common.Constants;
using System;
using System.Linq;
using System.Security.Claims;

namespace PhrasebookBackendService.EasyAuth
{
    public class AuthenticatedUser
    {
        private AuthenticatedUser(string email, string fullName, Guid principalId, string identityProvider)
        {
            this.Email = email ?? throw new ArgumentNullException(nameof(email));
            this.FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
            this.PrincipalId = principalId;
            this.IdentityProvider = identityProvider ?? throw new ArgumentNullException(identityProvider);
        }

        public string Email { get; }

        public string FullName { get; }

        public Guid PrincipalId { get; }

        public string IdentityProvider { get; }

        public static AuthenticatedUser FromClaimsPrincipal(ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null || claimsPrincipal.Identity == null)
            {
                throw new ArgumentNullException(nameof(claimsPrincipal), "Claims principal or its identity are null.");
            }

            Guid principalId = Guid.Parse(claimsPrincipal.Identity.Name);
            string identityProvider = claimsPrincipal.Identity.AuthenticationType;
            string userEmail = claimsPrincipal.Claims.Single(c => string.Equals(c.Type, Constants.EasyAuthEmailClaimType, StringComparison.OrdinalIgnoreCase)).Value.ToLowerInvariant();
            string userFullName = claimsPrincipal.Claims.Single(c => string.Equals(c.Type, Constants.EasyAuthFullNameClaimType, StringComparison.OrdinalIgnoreCase)).Value;

            return new AuthenticatedUser(userEmail, userFullName, principalId, identityProvider);
        }
    }
}
