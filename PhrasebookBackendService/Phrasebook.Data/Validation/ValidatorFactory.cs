using System;

namespace Phrasebook.Data.Validation
{
    public class ValidatorFactory : IValidatorFactory
    {
        private readonly PhrasebookDbContext context;

        public ValidatorFactory(PhrasebookDbContext context)
        {
            this.context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IGenericValidator CreateGenericValidator()
        {
            throw new NotImplementedException();
        }

        public IUserValidator CreateUserValidator()
        {
            return new UserValidator(this.context);
        }
    }
}
