using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Validation;

namespace PhrasebookBackendService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Constants.UserIsSignedUpPolicy)]
    [ApiController]
    public class UsersController : BaseController
    {
        public UsersController(
            ILogger<UsersController> logger,
            IUnitOfWork unitOfWork,
            IValidatorFactory validatorFactory,
            ITimeProvider timeProvider)
            : base(logger, unitOfWork, timeProvider, validatorFactory)
        {
        }

        [HttpGet("me")]
        [ActionName("GetAuthenticatedUserInformationAsync")]
        public async Task<IActionResult> GetAuthenticatedUserInformationAsync()
        {
            Phrasebook.Data.Models.User user = await this.UnitOfWork.UsersRepository.GetUserByPrincipalIdAsync(this.AuthenticatedUser.PrincipalId);
            return this.Ok(user.ToUserDto());
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut]
        public async Task<IActionResult> UpdateUserAsync([FromBody] string newDisplayName)
        {
            IUserValidator validator = this.ValidatorFactory.CreateUserValidator();
            if (!validator.IsValidDisplayName(newDisplayName))
            {
                return this.BadRequest($"Provided display name '{newDisplayName}' is not valid.");
            }

            Phrasebook.Data.Models.User updatedUser = await this.UnitOfWork.UsersRepository.UpdateUserDisplayNameAsync(this.AuthenticatedUser.PrincipalId, newDisplayName);
            this.Logger.LogInformation($"Updated user with ID {updatedUser.Id}");

            return this.Ok();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SignupUserAsync([FromBody] string displayName)
        {
            Guid principalId = this.AuthenticatedUser.PrincipalId;
            IUserValidator validator = this.ValidatorFactory.CreateUserValidator();
            if (await validator.HasUserSignedUpAsync(principalId))
            {
                return this.BadRequest($"User with principal ID '{principalId}' is already signed up.");
            }

            // Create new user entity
            this.Logger.LogInformation($"Creating new user with principal ID {principalId} from '{this.AuthenticatedUser.IdentityProvider}' Identity Provider");
            Phrasebook.Data.Models.User newUser = await this.UnitOfWork.UsersRepository
                .CreateNewUserAsync(this.AuthenticatedUser.IdentityProvider, principalId, this.AuthenticatedUser.Email, displayName, this.AuthenticatedUser.FullName);
            this.Logger.LogInformation($"Created new user with ID {newUser.Id}");

            return this.CreatedAtAction(nameof(GetAuthenticatedUserInformationAsync), newUser.ToUserDto());
        }

        // DELETE: api/Users
        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync()
        {
            Guid principalId = this.AuthenticatedUser.PrincipalId;
            this.Logger.LogInformation($"Deleting user with principal ID {principalId}");
            var user = await this.UnitOfWork.UsersRepository.DeleteUserAsync(principalId);
            this.Logger.LogInformation($"Successfully deleted user{Environment.NewLine}ID:{user.Id}{Environment.NewLine}Principal ID: {principalId}");

            return this.Ok();
        }
    }
}
