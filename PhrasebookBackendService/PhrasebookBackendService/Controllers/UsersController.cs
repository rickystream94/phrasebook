using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Dto.Models;

namespace PhrasebookBackendService.Controllers
{
    [Route("api/[controller]")]
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
            ITimeProvider timeProvider)
            : base(logger, context, timeProvider)
        {
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersAsync()
        {
            IEnumerable<Phrasebook.Data.Models.User> users = await this.DbContext
                .GetEntitiesAsync<Phrasebook.Data.Models.User>(navigationPropertiesToInclude: this.properties);
            this.Logger.LogInformation($"Retrieved {users.Count()} users.");
            return this.Ok(users
                .Select(u => u.ToUserDto())
                .ToListResult());
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        [ActionName("GetUserAsync")]
        public async Task<ActionResult<User>> GetUserAsync([FromRoute] int id)
        {
            var user = await this.DbContext.GetEntityByIdAsync(id, navigationPropertiesToInclude: this.properties);

            if (user == null)
            {
                return this.NotFound();
            }

            return this.Ok(user.ToUserDto());
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserAsync([FromRoute] int id, [FromBody] User user)
        {
            Phrasebook.Data.Models.User userToUpdate = await this.DbContext.GetEntityByIdAsync<Phrasebook.Data.Models.User>(id);
            if (userToUpdate == null)
            {
                return this.NotFound($"User with ID {id} was not found.");
            }

            // Only allow modifications to the nickname and email
            if (!string.IsNullOrWhiteSpace(user.DisplayName))
            {
                userToUpdate.DisplayName = user.DisplayName;
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                userToUpdate.Email = user.Email;
            }
            await this.DbContext.SaveChangesAsync();
            this.Logger.LogInformation($"Updated user with ID {userToUpdate.Id}");

            return this.Ok();
        }

        // POST: api/Users
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<User>> PostUserAsync([FromBody] User user)
        {
            if (user == null)
            {
                return this.BadRequest("User information not provided in the body.");
            }

            if ((await this.DbContext.GetEntityAsync<Phrasebook.Data.Models.User>(u => u.Email.ToLower() == user.Email.ToLower())) != null)
            {
                return this.BadRequest($"User with email {user.Email} already exists.");
            }

            Phrasebook.Data.Models.User newUser = new Phrasebook.Data.Models.User
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                SignedUpOn = this.TimeProvider.Now,
            };
            this.DbContext.Users.Add(newUser);
            await DbContext.SaveChangesAsync();
            this.Logger.LogInformation($"Created new user with ID {newUser.Id}");

            return this.CreatedAtAction(nameof(GetUserAsync), new { id = newUser.Id }, newUser.ToUserDto());
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserAsync([FromRoute] int id)
        {
            var user = await this.DbContext.GetEntityByIdAsync<Phrasebook.Data.Models.User>(id);
            if (user == null)
            {
                return this.NotFound();
            }

            this.DbContext.Users.Remove(user);
            await this.DbContext.SaveChangesAsync();
            this.Logger.LogInformation($"Deleted user with ID {user.Id}");

            return this.Ok();
        }
    }
}
