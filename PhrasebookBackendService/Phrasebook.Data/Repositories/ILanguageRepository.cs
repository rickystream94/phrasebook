using Phrasebook.Data.Models;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public interface ILanguageRepository : IGenericRepository<Language>
    {
        Task<Language> GetLanguageByCodeAsync(string languageCode);
    }
}
