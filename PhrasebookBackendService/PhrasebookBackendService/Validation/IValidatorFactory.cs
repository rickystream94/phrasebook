using System;
using Phrasebook.Data.Dto.Models.RequestData;

namespace PhrasebookBackendService.Validation
{
    public interface IValidatorFactory
    {
        IGenericValidator CreateOrUpdatePhrasebookValidator(Guid principalId, CreateOrUpdatePhrasebookRequestData requestData, int? existingPhrasebookId = null);

        IGenericValidator CreateOrUpdatePhraseValidator(Guid principalId, CreateOrUpdatePhraseRequestData requestData, int bookId, int ? existingPhraseId = null);
    }
}
