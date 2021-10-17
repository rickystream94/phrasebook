using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Validation;

namespace PhrasebookBackendService.Controllers
{
    [Route("api/[controller]")]
    [Authorize(Policy = Constants.UserIsSignedUpPolicy)]
    [ApiController]
    public class UsersController : BaseController
    {
        private readonly Expression<Func<Phrasebook.Data.Models.User, object>>[] properties = new Expression<Func<Phrasebook.Data.Models.User, object>>[]
        {
            u => u.Phrasebooks,
        };

        public UsersController(
            ILogger<UsersController> logger,
            PhrasebookDbContext context,
            IValidatorFactory validatorFactory,
            ITimeProvider timeProvider)
            : base(logger, context, timeProvider, validatorFactory)
        {
        }

        [HttpGet("me")]
        [ActionName("GetAuthenticatedUserInformationAsync")]
        public async Task<IActionResult> GetAuthenticatedUserInformationAsync()
        {
            Phrasebook.Data.Models.User user = await this.DbContext.GetEntityAsync<Phrasebook.Data.Models.User>(u => u.PrincipalId == this.AuthenticatedUser.PrincipalId);
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

            Phrasebook.Data.Models.User userToUpdate = await this.DbContext.GetEntityAsync<Phrasebook.Data.Models.User>(u => u.PrincipalId == this.AuthenticatedUser.PrincipalId);
            userToUpdate.DisplayName = newDisplayName;
            await this.DbContext.SaveChangesAsync();
            this.Logger.LogInformation($"Updated user with ID {userToUpdate.Id}");

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
            Phrasebook.Data.Models.User newUser = new Phrasebook.Data.Models.User
            {
                IdentityProvider = this.AuthenticatedUser.IdentityProvider,
                PrincipalId = principalId,
                Email = this.AuthenticatedUser.Email,
                DisplayName = displayName,
                FullName = this.AuthenticatedUser.FullName,
                SignedUpOn = this.TimeProvider.Now,
            };
            this.DbContext.Users.Add(newUser);
            await DbContext.SaveChangesAsync();
            this.Logger.LogInformation($"Created new user with ID {newUser.Id}");

            return this.CreatedAtAction(nameof(GetAuthenticatedUserInformationAsync), newUser.ToUserDto());
        }

        // DELETE: api/Users/5
        [HttpDelete]
        public async Task<IActionResult> DeleteUserAsync()
        {
            var user = await this.DbContext.GetEntityAsync<Phrasebook.Data.Models.User>(u => u.PrincipalId == this.AuthenticatedUser.PrincipalId);
            this.DbContext.Users.Remove(user);
            await this.DbContext.SaveChangesAsync();
            this.Logger.LogInformation($"Deleted user with ID {user.Id}");

            return this.Ok();
        }
    }
}
