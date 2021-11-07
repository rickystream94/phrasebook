using PhrasebookBackendService.Exceptions;

namespace PhrasebookBackendService.Validation
{
    public static class UserNameValidator
    {
        public static void ValidateDisplayName(string displayName)
        {
            if (string.IsNullOrWhiteSpace(displayName))
            {
                throw new InputValidationException($"Provided display name '{displayName}' is not valid.");
            }

            // TODO: add more validation (e.g. is valid alphanumeric string)
        }
    }
}
