using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Models;

namespace PhrasebookBackendService.Validation
{
    public interface IValidatorFactory
    {
        IUserValidator CreateUserValidator();

        IGenericValidator CreatePhrasebookValidator(User user, CreatePhrasebookRequestData requestData);
    }
}
