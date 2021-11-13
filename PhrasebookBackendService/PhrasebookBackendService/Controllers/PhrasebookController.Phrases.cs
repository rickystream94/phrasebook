using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Phrasebook.Data.Dto;
using Phrasebook.Data.Dto.Models.RequestData;
using PhrasebookBackendService.Exceptions;
using PhrasebookBackendService.Validation;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Controllers
{
    public partial class PhrasebookController
    {
        [HttpGet("{bookId}/phrases/{phraseId}")]
        [ActionName("GetPhraseAsync")]
        public async Task<IActionResult> GetPhraseAsync([FromRoute] int bookId, [FromRoute] int phraseId)
        {
            Phrasebook.Data.Models.Phrase phrase = await this.UnitOfWork.PhraseRepository.GetPhraseAsync(bookId, phraseId, this.AuthenticatedUser.PrincipalId);

            if (phrase == null)
            {
                return this.NotFound();
            }

            return Ok(phrase.ToPhraseDto());
        }

        [HttpPost("{bookId}/phrases")]
        public async Task<IActionResult> CreatePhraseAsync([FromRoute] int bookId, [FromBody] CreateOrUpdatePhraseRequestData requestData)
        {
            // Validate input
            IGenericValidator validator = this.ValidatorFactory.CreateOrUpdatePhraseValidator(this.AuthenticatedUser.PrincipalId, requestData, bookId);
            try
            {
                await validator.ValidateAsync();
            }
            catch (InputValidationException ex)
            {
                this.Logger.LogError(ex.Message);
                return this.BadRequest(ex.Message);
            }

            // Validation passed: create phrase
            this.Logger.LogInformation($"Creating new phrase in phrasebook with ID {bookId} for user with principal ID '{this.AuthenticatedUser.PrincipalId}'");
            Phrasebook.Data.Models.Phrase newPhrase = await this.UnitOfWork.PhraseRepository.CreatePhraseAsync(
                bookId,
                this.AuthenticatedUser.PrincipalId,
                requestData.FirstLanguagePhrase,
                requestData.ForeignLanguagePhrase,
                (Phrasebook.Data.Models.LexicalItemType)requestData.LexicalItemType,
                requestData.ForeignLanguageSynonyms,
                requestData.Description);

            return this.CreatedAtAction(nameof(GetPhraseAsync), new { bookId, phraseId = newPhrase.Id }, newPhrase.ToPhraseDto());
        }

        [HttpPut("{bookId}/phrases/{phraseId}")]
        public async Task<IActionResult> UpdatePhraseAsync([FromRoute] int bookId, [FromRoute] int phraseId, [FromBody] CreateOrUpdatePhraseRequestData requestData)
        {
            // Validate input
            IGenericValidator validator = this.ValidatorFactory.CreateOrUpdatePhraseValidator(this.AuthenticatedUser.PrincipalId, requestData, bookId, phraseId);
            try
            {
                await validator.ValidateAsync();
            }
            catch (InputValidationException ex)
            {
                this.Logger.LogError(ex.Message);
                return this.BadRequest(ex.Message);
            }

            // Validation passed: update phrase
            this.Logger.LogInformation($"Updating phrase with ID {phraseId} in phrasebook with ID {bookId} for user with principal ID '{this.AuthenticatedUser.PrincipalId}'");
            Phrasebook.Data.Models.Phrase updatedPhrase = await this.UnitOfWork.PhraseRepository.UpdatePhraseAsync(
                bookId,
                phraseId,
                this.AuthenticatedUser.PrincipalId,
                requestData.FirstLanguagePhrase,
                requestData.ForeignLanguagePhrase,
                (Phrasebook.Data.Models.LexicalItemType?)requestData.LexicalItemType,
                requestData.ForeignLanguageSynonyms,
                requestData.Description);
            return this.Ok(updatedPhrase.ToPhraseDto());
        }

        [HttpDelete("{bookId}/phrases/{phraseId}")]
        public async Task<IActionResult> DeletePhraseAsync([FromRoute] int bookId, [FromRoute] int phraseId)
        {
            Phrasebook.Data.Models.Phrase existingPhrase = await this.UnitOfWork.PhraseRepository.GetPhraseAsync(bookId, phraseId, this.AuthenticatedUser.PrincipalId);
            if (existingPhrase == null)
            {
                return this.BadRequest($"Could not find phrase with ID {phraseId} in phrasebook with ID {bookId} for the user with principal ID {this.AuthenticatedUser.PrincipalId}");
            }

            await this.UnitOfWork.PhraseRepository.DeletePhraseAsync(bookId, phraseId, this.AuthenticatedUser.PrincipalId);
            return this.Ok();
        }
    }
}
