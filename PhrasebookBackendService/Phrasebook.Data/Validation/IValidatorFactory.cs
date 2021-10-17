namespace Phrasebook.Data.Validation
{
    public interface IValidatorFactory
    {
        IUserValidator CreateUserValidator();

        IGenericValidator CreateGenericValidator();
    }
}
