using System;
using System.Collections.Generic;

namespace PhrasebookBackendService.Exceptions
{
    public class InputValidationException : Exception
    {
        private const string DefaultErrorMessageFormat = "Input validation failed. Errors: {0}";

        public InputValidationException(string message)
            : base(message)
        {
        }

        public InputValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InputValidationException(List<string> errors)
            : this(errors, null)
        {
        }

        public InputValidationException(List<string> errors, Exception innerException)
            : base(string.Format(DefaultErrorMessageFormat, string.Join(Environment.NewLine, errors)), innerException)
        {
            this.Errors = errors.ToArray();
        }

        public string[] Errors { get; }
    }
}
