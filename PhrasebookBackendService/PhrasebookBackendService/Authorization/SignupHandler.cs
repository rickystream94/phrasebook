using Microsoft.AspNetCore.Authorization;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.EasyAuth;
using System;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Authorization
{
    public class SignupHandler : AuthorizationHandler<SignupRequirement>
    {
        private readonly IUnitOfWork unitOfWork;

        public SignupHandler(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SignupRequirement requirement)
        {
            // The requirement is met only if a registered user already exists in the database, with the same principal ID of the authenticated user
            AuthenticatedUser authenticatedUser = AuthenticatedUser.FromClaimsPrincipal(context.User);
            User user = await this.unitOfWork.UserRepository.GetUserByPrincipalIdAsync(authenticatedUser.PrincipalId);
            if (user != null)
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
