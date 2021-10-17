using Microsoft.AspNetCore.Authorization;
using Phrasebook.Data.Validation;
using PhrasebookBackendService.EasyAuth;
using System;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Authorization
{
    public class SignupHandler : AuthorizationHandler<SignupRequirement>
    {
        private readonly IValidatorFactory validatorFactory;

        public SignupHandler(IValidatorFactory validatorFactory)
        {
            this.validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SignupRequirement requirement)
        {
            AuthenticatedUser user = AuthenticatedUser.FromClaimsPrincipal(context.User);
            IUserValidator userValidator = this.validatorFactory.CreateUserValidator();
            if (await userValidator.HasUserSignedUpAsync(user.PrincipalId))
            {
                context.Succeed(requirement);
                return;
            }

            context.Fail();
        }
    }
}
