using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Dto.Models;
using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Validation;

namespace PhrasebookBackendService.Controllers
{
    [ApiController]
    [Authorize(Policy = Constants.UserIsSignedUpPolicy)]
    [Route("api/[controller]")]
    public class PhrasebooksController : BaseController
    {
        private readonly Expression<Func<Phrasebook.Data.Models.Book, object>>[] properties = new Expression<Func<Phrasebook.Data.Models.Book, object>>[]
        {
            b => b.FirstLanguage,
            b => b.ForeignLanguage,
            b => b.User
        };

        private readonly Expression<Func<Phrasebook.Data.Models.Book, object>>[] extendedProperties = new Expression<Func<Phrasebook.Data.Models.Book, object>>[]
        {
            b => b.FirstLanguage,
            b => b.ForeignLanguage,
            b => b.User,
            b => b.Phrases,
        };

        public PhrasebooksController(
            ILogger<PhrasebooksController> logger,
            PhrasebookDbContext dbContext,
            ITimeProvider timeProvider,
            IValidatorFactory validatorFactory)
            : base(logger, dbContext, timeProvider, validatorFactory)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ListResult<Book>>> GetPhrasebooksAsync()
        {
            IEnumerable<Phrasebook.Data.Models.Book> phrasebooks = await this.DbContext
                .GetEntitiesAsync(p => p.User.Email == this.AuthenticatedUser.Email, navigationPropertiesToInclude: this.properties);
            return Ok(phrasebooks
                .Select(b => b.ToPhrasebookDto())
                .ToListResult());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhrasebookAsync([FromRoute] int id)
        {
            Phrasebook.Data.Models.Book phrasebook = await this.DbContext
                .GetEntityAsync(p => p.User.Email == this.AuthenticatedUser.Email && p.Id == id, this.extendedProperties);

            if (phrasebook == null)
            {
                return this.NotFound();
            }

            return Ok(phrasebook.ToPhrasebookDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhrasebookAsync([FromBody] CreatePhrasebookRequestData requestData)
        {
            // Validate selected language codes exist
            if (string.IsNullOrWhiteSpace(requestData?.FirstLanguageCode) || string.IsNullOrWhiteSpace(requestData?.ForeignLanguageCode))
            {
                return this.BadRequest("Invalid request data.");
            }

            string firstLanguageCode = requestData.FirstLanguageCode.ToLower();
            string foreignLanguageCode = requestData.ForeignLanguageCode.ToLower();
            Phrasebook.Data.Models.Language firstLanguage = await this.DbContext.GetEntityAsync<Phrasebook.Data.Models.Language>(l => l.Code == firstLanguageCode);
            if (firstLanguage == null)
            {
                return this.BadRequest($"Language with code '{firstLanguageCode}' was not found.");
            }

            Phrasebook.Data.Models.Language foreignLanguage = await this.DbContext.GetEntityAsync<Phrasebook.Data.Models.Language>(l => l.Code == foreignLanguageCode);
            if (foreignLanguage == null)
            {
                return this.BadRequest($"Language with code '{foreignLanguageCode}' was not found.");
            }

            // Validate that first and foreign language codes are not the same
            if (firstLanguage.Id == foreignLanguage.Id)
            {
                return this.BadRequest("The provided first and foreign language codes are equal.");
            }

            // Validate that a phrasebook for the authenticated user with the same language codes doesn't already exist
            Expression<Func<Phrasebook.Data.Models.Book, bool>> bookFilter = p => p.FirstLanguage.Code == firstLanguageCode &&
                    p.ForeignLanguage.Code == foreignLanguageCode &&
                    p.User.Email == this.AuthenticatedUser.Email;
            Phrasebook.Data.Models.Book existingBook = await this.DbContext.GetEntityAsync(bookFilter, this.properties);
            if (existingBook != null)
            {
                return this.BadRequest($"A phrasebook with first language code '{firstLanguageCode}' and foreign language code '{foreignLanguageCode}' already exists.");
            }

            // Validation passed: create new phrasebook
            Phrasebook.Data.Models.User user = await this.DbContext
                .GetEntityAsync<Phrasebook.Data.Models.User>(u => u.PrincipalId == this.AuthenticatedUser.PrincipalId);
            Phrasebook.Data.Models.Book bookToCreate = new Phrasebook.Data.Models.Book
            {
                CreatedOn = this.TimeProvider.Now,
                FirstLanguageId = firstLanguage.Id,
                ForeignLanguageId = foreignLanguage.Id,
                UserId = user.Id,
            };
            this.DbContext.Phrasebooks.Add(bookToCreate);
            await this.DbContext.SaveChangesAsync();

            // Get newly created phrasebook from the DB
            Phrasebook.Data.Models.Book newBook = await this.DbContext.GetEntityAsync(bookFilter, this.properties);
            return this.Ok(newBook.ToPhrasebookDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhrasebookAsync([FromRoute] int id)
        {
            // Validate that book exists
            Phrasebook.Data.Models.Book existingBook = await this.DbContext
                .GetEntityAsync<Phrasebook.Data.Models.Book>(p => p.Id == id && p.User.Email == this.AuthenticatedUser.Email, p => p.User);
            if (existingBook == null)
            {
                return this.BadRequest($"Could not find phrasebook with ID '{id}' for the authenticated user.");
            }

            this.DbContext.Phrasebooks.Remove(existingBook);
            await this.DbContext.SaveChangesAsync();
            return this.Ok();
        }
    }
}
