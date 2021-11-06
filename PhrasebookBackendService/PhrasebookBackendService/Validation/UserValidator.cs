using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Validation
{
    public class UserValidator : IUserValidator
    {
        private readonly IUnitOfWork unitOfWork;

        public UserValidator(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<bool> HasUserSignedUpAsync(Guid principalId)
        {
            User user = await this.unitOfWork.UsersRepository.GetEntityAsync(u => u.PrincipalId == principalId);
            return user != null;
        }

        public bool IsValidDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                return false;
            }

            // TODO: add more validation (e.g. is valid alphanumeric string)

            return true;
        }
    }
}
