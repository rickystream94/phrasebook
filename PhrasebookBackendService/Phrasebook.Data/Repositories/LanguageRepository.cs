using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;

namespace Phrasebook.Data.Repositories
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(PhrasebookDbContext context)
            : base(context)
        {
        }
    }
}
