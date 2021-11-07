using System.Threading.Tasks;

namespace PhrasebookBackendService.Validation
{
    public interface IGenericValidator
    {
        Task<bool> ValidateAsync();
    }
}
