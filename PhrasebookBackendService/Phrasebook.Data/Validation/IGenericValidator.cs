using System.Threading.Tasks;

namespace Phrasebook.Data.Validation
{
    public interface IGenericValidator
    {
        Task<bool> ValidateAsync();
    }
}
