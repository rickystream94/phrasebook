using System;
using System.Threading.Tasks;

namespace PhrasebookBackendService.Validation
{
    public interface IUserValidator
    {
        Task<bool> HasUserSignedUpAsync(Guid principalId);

        bool IsValidDisplayName(string displayName);
    }
}
