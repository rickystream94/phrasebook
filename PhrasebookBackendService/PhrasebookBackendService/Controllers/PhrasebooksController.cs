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
    public class PhrasebooksController : BaseController
    {
        public PhrasebooksController(
            ILogger<PhrasebooksController> logger,
            IUnitOfWork unitOfWork,
            ITimeProvider timeProvider,
            IValidatorFactory validatorFactory)
            : base(logger, unitOfWork, timeProvider, validatorFactory)
        {
        }

        [HttpGet]
        public async Task<ActionResult<ListResult<Book>>> GetPhrasebooksAsync()
        {
            IEnumerable<Phrasebook.Data.Models.Book> phrasebooks = await this.UnitOfWork.BooksRepository.GetUserPhrasebooksAsync(this.AuthenticatedUser.PrincipalId);
            return Ok(phrasebooks
                .Select(b => b.ToPhrasebookDto())
                .ToListResult());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPhrasebookAsync([FromRoute] int id)
        {
            Phrasebook.Data.Models.Book phrasebook = await this.UnitOfWork.BooksRepository.GetUserPhrasebookByIdAsync(this.AuthenticatedUser.PrincipalId, id);

            if (phrasebook == null)
            {
                return this.NotFound();
            }

            return Ok(phrasebook.ToPhrasebookDto());
        }

        [HttpPost]
        public async Task<IActionResult> CreatePhrasebookAsync([FromBody] CreatePhrasebookRequestData requestData)
        {
            // Validate input
            Phrasebook.Data.Models.User user = await this.UnitOfWork.UsersRepository.GetUserByPrincipalIdAsync(this.AuthenticatedUser.PrincipalId);
            IGenericValidator validator = this.ValidatorFactory.CreatePhrasebookValidator(user, requestData);
            try
            {
                await validator.ValidateAsync();
            }
            catch (InputValidationException ex)
            {
                return this.BadRequest(ex);
            }

            // Validation passed: create new phrasebook
            this.Logger.LogInformation($"Creating new phrasebook for user with principal ID '{this.AuthenticatedUser.PrincipalId}'{Environment.NewLine}First language code: {requestData.FirstLanguageCode}{Environment.NewLine}Foreign language code: {requestData.ForeignLanguageCode}");
            Phrasebook.Data.Models.Language firstLanguage = await this.UnitOfWork.LanguageRepository.GetLanguageByCodeAsync(requestData.FirstLanguageCode);
            Phrasebook.Data.Models.Language foreignLanguage = await this.UnitOfWork.LanguageRepository.GetLanguageByCodeAsync(requestData.ForeignLanguageCode);
            Phrasebook.Data.Models.Book newBook = await this.UnitOfWork.BooksRepository.CreateNewPhrasebookAsync(firstLanguage, foreignLanguage, user);
            return this.Ok(newBook.ToPhrasebookDto());
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhrasebookAsync([FromRoute] int id)
        {
            // Validate that book exists
            Phrasebook.Data.Models.Book existingBook = await this.UnitOfWork.BooksRepository
                .GetUserPhrasebookByIdAsync(this.AuthenticatedUser.PrincipalId, id, p => p.User);
            if (existingBook == null)
            {
                return this.BadRequest($"Could not find phrasebook with ID '{id}' for the authenticated user.");
            }

            this.UnitOfWork.BooksRepository.Delete(existingBook);
            await this.UnitOfWork.SaveChangesAsync();
            return this.Ok();
        }
    }
}
