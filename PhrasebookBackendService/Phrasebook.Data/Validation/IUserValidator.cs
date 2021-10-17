using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Validation
{
    public interface IUserValidator
    {
        Task<bool> HasUserSignedUpAsync(Guid principalId);

        bool IsValidDisplayName(string displayName);
    }
}
