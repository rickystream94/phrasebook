using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Common;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Dto.Models;
using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Exceptions;
using PhrasebookBackendService.Validation;

namespace PhrasebookBackendService.Controllers
{
    [ApiController]
    [Authorize(Policy = Constants.UserIsSignedUpPolicy)]
    [Route("api/[controller]")]
    public partial class PhrasebookController : BaseController
    {
        public PhrasebookController(
            ILogger<PhrasebookController> logger,
            IUnitOfWork unitOfWork,
            ITimeProvider timeProvider,
            IValidatorFactory validatorFactory)
            : base(logger, unitOfWork, timeProvider, validatorFactory)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ListResult<Book>>> GetPhrasebooksAsync()
        {
            IEnumerable<Phrasebook.Data.Models.Book> phrasebooks = await this.UnitOfWork.BookRepository.GetPhrasebooksAsync(this.AuthenticatedUser.PrincipalId);
            return Ok(phrasebooks
                .Select(b => b.ToPhrasebookDto())
                .ToListResult());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhrasebookAsync([FromRoute] int id)
        {
            Phrasebook.Data.Models.Book phrasebook = await this.UnitOfWork.BookRepository.GetPhrasebookByIdAsync(this.AuthenticatedUser.PrincipalId, id, true);

            if (phrasebook == null)
            {
                return this.NotFound();
            }

            return Ok(phrasebook.ToPhrasebookDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhrasebookAsync([FromBody] CreateOrUpdatePhrasebookRequestData requestData)
        {
            // Validate input
            IGenericValidator validator = this.ValidatorFactory.CreateOrUpdatePhrasebookValidator(this.AuthenticatedUser.PrincipalId, requestData);
            try
            {
                await validator.ValidateAsync();
            }
            catch (InputValidationException ex)
            {
                this.Logger.LogError(ex.Message);
                return this.BadRequest(ex.Message);
            }

            // Validation passed: create new phrasebook
            this.Logger.LogInformation(string.Join(Environment.NewLine, new string[]
            {
                $"Creating phrasebook for user with principal ID '{this.AuthenticatedUser.PrincipalId}' with the following request data:",
                $"First language code: {requestData.FirstLanguageCode}",
                $"Foreign language code: {requestData.ForeignLanguageCode}"
            }));
            Phrasebook.Data.Models.User user = await this.UnitOfWork.UserRepository.GetUserByPrincipalIdAsync(this.AuthenticatedUser.PrincipalId);
            Phrasebook.Data.Models.Language firstLanguage = await this.UnitOfWork.LanguageRepository.GetLanguageByCodeAsync(requestData.FirstLanguageCode);
            Phrasebook.Data.Models.Language foreignLanguage = await this.UnitOfWork.LanguageRepository.GetLanguageByCodeAsync(requestData.ForeignLanguageCode);
            Phrasebook.Data.Models.Book newBook = await this.UnitOfWork.BookRepository.CreateNewPhrasebookAsync(firstLanguage, foreignLanguage, user);
            return this.Ok(newBook.ToPhrasebookDto());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePhrasebookAsync([FromRoute] int id, [FromBody] CreateOrUpdatePhrasebookRequestData requestData)
        {
            // Validate input
            IGenericValidator validator = this.ValidatorFactory.CreateOrUpdatePhrasebookValidator(this.AuthenticatedUser.PrincipalId, requestData, id) ;
            try
            {
                await validator.ValidateAsync();
            }
            catch (InputValidationException ex)
            {
                this.Logger.LogError(ex.Message);
                return this.BadRequest(ex.Message);
            }

            // Validation passed: update existing phrasebook
            this.Logger.LogInformation(string.Join(Environment.NewLine, new string[]
            {
                $"Updating phrasebook with ID {id} for user with principal ID '{this.AuthenticatedUser.PrincipalId}' with the following request data:",
                $"{requestData}",
            }));
            Phrasebook.Data.Models.Language firstLanguage = await this.UnitOfWork.LanguageRepository.GetLanguageByCodeAsync(requestData.FirstLanguageCode);
            Phrasebook.Data.Models.Language foreignLanguage = await this.UnitOfWork.LanguageRepository.GetLanguageByCodeAsync(requestData.ForeignLanguageCode);
            Phrasebook.Data.Models.Book updatedBook = await this.UnitOfWork.BookRepository.UpdatePhrasebookAsync(this.AuthenticatedUser.PrincipalId, id, firstLanguage, foreignLanguage);
            return this.Ok(updatedBook.ToPhrasebookDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhrasebookAsync([FromRoute] int id)
        {
            // Validate that book exists
            Phrasebook.Data.Models.Book existingBook = await this.UnitOfWork.BookRepository.GetPhrasebookByIdAsync(this.AuthenticatedUser.PrincipalId, id);
            if (existingBook == null)
            {
                return this.BadRequest($"Could not find phrasebook with ID '{id}' for the authenticated user.");
            }

            await this.UnitOfWork.BookRepository.DeletePhrasebookAsync(this.AuthenticatedUser.PrincipalId, id);
            return this.Ok();
        }
    }
}
