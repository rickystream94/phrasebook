using Phrasebook.Data.Models;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Validation
{
    public class UserValidator : IUserValidator
    {
        private PhrasebookDbContext context;

        public UserValidator(PhrasebookDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> HasUserSignedUpAsync(Guid principalId)
        {
            User user = await this.context.GetEntityAsync<User>(u => u.PrincipalId == principalId);
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
