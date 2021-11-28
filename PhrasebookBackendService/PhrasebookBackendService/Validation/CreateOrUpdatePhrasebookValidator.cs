using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Exceptions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Validation
{
    public class CreateOrUpdatePhrasebookValidator : IGenericValidator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly CreateOrUpdatePhrasebookRequestData requestData;
        private readonly Guid principalId;
        private readonly int? existingPhrasebookId;


        public CreateOrUpdatePhrasebookValidator(IUnitOfWork unitOfWork, Guid principalId, CreateOrUpdatePhrasebookRequestData requestData, int? existingPhrasebookId = null)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.requestData = requestData ?? throw new InputValidationException("Provided request data is null");
            this.principalId = principalId;
            this.existingPhrasebookId = existingPhrasebookId;
        }

        public async Task<bool> ValidateAsync()
        {
            // Validate request data
            if (string.IsNullOrWhiteSpace(this.requestData.FirstLanguageCode) || string.IsNullOrWhiteSpace(this.requestData.ForeignLanguageCode))
            {
                throw new InputValidationException("Invalid request data. Please check your input.");
            }

            // Validate selected language codes exist
            string firstLanguageCode = requestData.FirstLanguageCode.ToLower();
            string foreignLanguageCode = requestData.ForeignLanguageCode.ToLower();
            Language firstLanguage = await this.unitOfWork.LanguageRepository.GetLanguageByCodeAsync(firstLanguageCode);
            if (firstLanguage == null)
            {
                throw new InputValidationException($"Language with code '{firstLanguageCode}' was not found.");
            }

            Language foreignLanguage = await this.unitOfWork.LanguageRepository.GetLanguageByCodeAsync(foreignLanguageCode);
            if (foreignLanguage == null)
            {
                throw new InputValidationException($"Language with code '{foreignLanguageCode}' was not found.");
            }

            List<string> errors = new List<string>();

            // Validate that first and foreign language codes are not the same
            if (firstLanguage.Id == foreignLanguage.Id)
            {
                errors.Add("The provided first and foreign language codes are equal.");
            }

            Book existingBook;
            if (!this.existingPhrasebookId.HasValue)
            {
                // If creating new phrasebook: validate that a phrasebook for the authenticated user with the same languages doesn't already exist
                existingBook = await this.unitOfWork.BookRepository.GetPhrasebookByLanguagesAsync(this.principalId, firstLanguage, foreignLanguage);
                if (existingBook != null)
                {
                    errors.Add($"A phrasebook with first language code '{firstLanguage.Id}' and foreign language code '{foreignLanguage.Id}' for user with Principal ID '{this.principalId}' already exists.");
                }
            }
            else
            {
                // If updating existing phrasebook: validate that a phrasebook with the specified ID already exists
                existingBook = await this.unitOfWork.BookRepository.GetPhrasebookByIdAsync(this.principalId, this.existingPhrasebookId.Value);
                if (existingBook == null)
                {
                    errors.Add($"Could not find an existing phrasebook with ID {this.existingPhrasebookId.Value} for user with Principal ID {this.principalId}");
                }
            }

            if (errors.Count != 0)
            {
                throw new InputValidationException(errors);
            }

            return true;
        }
    }
}
