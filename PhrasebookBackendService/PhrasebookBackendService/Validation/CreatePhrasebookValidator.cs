using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using PhrasebookBackendService.Exceptions;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Validation
{
    public class CreatePhrasebookValidator : IGenericValidator
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly CreatePhrasebookRequestData requestData;
        private readonly User user;


        public CreatePhrasebookValidator(IUnitOfWork unitOfWork, User user, CreatePhrasebookRequestData requestData)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            this.user = user ?? throw new ArgumentNullException(nameof(user));
            this.requestData = requestData ?? throw new InputValidationException("Provided request data is null");
        }

        public async Task ValidateAsync()
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

            // Validate that first and foreign language codes are not the same
            if (firstLanguage.Id == foreignLanguage.Id)
            {
                throw new InputValidationException("The provided first and foreign language codes are equal.");
            }

            // Validate that a phrasebook for the authenticated user with the same languages doesn't already exist
            Book existingBook = await this.unitOfWork.BooksRepository.GetUserPhrasebookByLanguagesAsync(this.user.PrincipalId, firstLanguage, foreignLanguage);
            if (existingBook != null)
            {
                throw new InputValidationException($"A phrasebook with first language code '{firstLanguage.Code}' and foreign language code '{foreignLanguage.Code}' for user with Principal ID '{user.PrincipalId}' already exists.");
            }
        }
    }
}
