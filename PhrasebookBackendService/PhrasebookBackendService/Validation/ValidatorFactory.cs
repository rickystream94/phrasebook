using Phrasebook.Data.Dto.Models.RequestData;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;

namespace PhrasebookBackendService.Validation
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly IUnitOfWork unitOfWork;

        public ValidatorFactory(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public IUserValidator CreateUserValidator()
        {
            return new UserValidator(this.unitOfWork);
        }

        public IGenericValidator CreatePhrasebookValidator(User user, CreatePhrasebookRequestData requestData)
        {
            return new CreatePhrasebookValidator(this.unitOfWork, user, requestData);
        }
    }
}
