using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Authorization
{
    public class SignupAuthenticationHandler : IAuthenticationHandler
    {
        private HttpContext context;

        public Task InitializeAsync(AuthenticationScheme scheme, HttpContext context)
        {
            this.context = context;
            return Task.CompletedTask;
        }

        public Task<AuthenticateResult> AuthenticateAsync() => Task.FromResult(AuthenticateResult.NoResult());

        public Task ChallengeAsync(AuthenticationProperties properties) => Task.FromResult(AuthenticateResult.NoResult());

        public Task ForbidAsync(AuthenticationProperties properties)
        {
            this.context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            return Task.CompletedTask;
        }
    }
}
