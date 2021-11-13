using System;
using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Sql;

namespace PhrasebookBackendService.Validation
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IUnitOfWork unitOfWork;

        public ValidatorFactory(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IGenericValidator CreateOrUpdatePhrasebookValidator(Guid principalId, CreateOrUpdatePhrasebookRequestData requestData, int? existingPhrasebookId = null)
        {
            return new CreateOrUpdatePhrasebookValidator(this.unitOfWork, principalId, requestData, existingPhrasebookId);
        }

        public IGenericValidator CreateOrUpdatePhraseValidator(Guid principalId, CreateOrUpdatePhraseRequestData requestData, int bookId, int ? existingPhraseId = null)
        {
            return new CreateOrUpdatePhraseValidator(this.unitOfWork, principalId, requestData, bookId, existingPhraseId);
        }
    }
}
