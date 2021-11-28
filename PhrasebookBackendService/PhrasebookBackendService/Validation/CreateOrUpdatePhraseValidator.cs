using Phrasebook.Common.Constants;
using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Validation
{
    public class CreateOrUpdatePhraseValidator : IGenericValidator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly CreateOrUpdatePhraseRequestData requestData;
        private readonly Guid principalId;
        private readonly int bookId;
        private readonly int? existingPhraseId;
        private readonly Regex emptyOrWhitespaceStringRegex = new Regex("^\\s*$"); 

        public CreateOrUpdatePhraseValidator(IUnitOfWork unitOfWork, Guid principalId, CreateOrUpdatePhraseRequestData requestData, int bookId, int? existingPhraseId = null)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.requestData = requestData ?? throw new InputValidationException("Provided request data is null");
            this.principalId = principalId;
            this.bookId = bookId;
            this.existingPhraseId = existingPhraseId;
        }

        public async Task<bool> ValidateAsync()
        {
            // Validate that provided phrasebook ID exists
            Book book = await this.unitOfWork.BookRepository.GetPhrasebookByIdAsync(this.principalId, this.bookId, true);
            if (book == null)
            {
                throw new InputValidationException($"Phrasebook with ID {this.bookId} cannot be found for user with Principal ID {this.principalId}");
            }

            List<string> errors = new List<string>();

            if (!this.existingPhraseId.HasValue)
            {
                // Creating a new phrase
                // Both first and foreign language phrases must be non null
                if (string.IsNullOrWhiteSpace(this.requestData.FirstLanguagePhrase) || string.IsNullOrWhiteSpace(this.requestData.ForeignLanguagePhrase))
                {
                    errors.Add($"{nameof(this.requestData.FirstLanguagePhrase)} or {nameof(this.requestData.ForeignLanguagePhrase)} were not provided or are empty.");
                }

                if (!this.requestData.LexicalItemType.HasValue)
                {
                    errors.Add($"{nameof(this.requestData.LexicalItemType)} must be provided when creating a new phrase.");
                }

                // Validate that no other phrase records exist in the same phrasebook, that match either the first or foreign language phrase
                if (book.Phrases.Where(
                    p => p.FirstLanguagePhrase == this.requestData.FirstLanguagePhrase.ToLowerInvariant() ||
                    p.ForeignLanguagePhrase == this.requestData.ForeignLanguagePhrase.ToLowerInvariant()).Any())
                {
                    errors.Add($"A phrase already exists with the same {nameof(this.requestData.FirstLanguagePhrase)} or {nameof(this.requestData.ForeignLanguagePhrase)} in phrasebook with ID {this.bookId} for user with principal ID {this.principalId}.");
                }
            }
            else
            {
                // Validate that the phrase exists and is linked to a phrasebook that belongs to the user
                Phrase phrase = await this.unitOfWork.PhraseRepository.GetPhraseAsync(this.bookId, this.existingPhraseId.Value, this.principalId);
                if (phrase == null || phrase.Phrasebook.User.PrincipalId != this.principalId)
                {
                    errors.Add($"Could not find a phrase with ID {this.existingPhraseId.Value} in phrasebook with ID {this.bookId} for user with Principal ID {this.principalId}");
                }

                // Validate that, if first/foreign language phrases are being updated, they are not empty strings
                if (this.requestData.FirstLanguagePhrase != null && emptyOrWhitespaceStringRegex.IsMatch(this.requestData.FirstLanguagePhrase))
                {
                    errors.Add($"{nameof(this.requestData.FirstLanguagePhrase)} must not be empty.");
                }

                if (this.requestData.ForeignLanguagePhrase != null && emptyOrWhitespaceStringRegex.IsMatch(this.requestData.ForeignLanguagePhrase))
                {
                    errors.Add($"{nameof(this.requestData.ForeignLanguagePhrase)} must not be empty.");
                }
            }

            if (this.requestData.FirstLanguagePhrase.Length >= Constants.MaxPhraseLength || this.requestData.ForeignLanguagePhrase.Length >= Constants.MaxPhraseLength)
            {
                errors.Add($"{nameof(this.requestData.FirstLanguagePhrase)} or {nameof(this.requestData.ForeignLanguagePhrase)} exceed max allowed length of {Constants.MaxPhraseLength} characters.");
            }

            if (string.Join(';', this.requestData.ForeignLanguageSynonyms).Length >= Constants.MaxSynonymsLength)
            {
                errors.Add($"{nameof(this.requestData.ForeignLanguageSynonyms)} exceed max allowed joined string length of {Constants.MaxSynonymsLength} 500 characters.");
            }

            if (errors.Count != 0)
            {
                throw new InputValidationException(errors);
            }

            return true;
        }
    }
}