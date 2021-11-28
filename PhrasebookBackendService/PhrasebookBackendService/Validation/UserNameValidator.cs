using Phrasebook.Common;
using PhrasebookBackendService.Exceptions;

namespace PhrasebookBackendService.Validation
{
    public static class UserNameValidator
    {
        public static void ValidateDisplayName(string displayName)
        {
            if (!displayName.IsValidDisplayName())
            {
                throw new InputValidationException($"Provided display name '{displayName}' is not valid.");
            }
        }
    }
}
