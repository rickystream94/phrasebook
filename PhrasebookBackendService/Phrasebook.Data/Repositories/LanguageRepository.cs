using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public class LanguageRepository : GenericRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(PhrasebookDbContext context)
            : base(context)
        {
        }

        public async Task<Language> GetLanguageByCodeAsync(string languageCode)
        {
            return await this.GetEntityAsync(l => l.Code == languageCode.ToLower());
        }

        protected override Expression<Func<Language, object>>[] GetCommonNavigationProperties()
        {
            return Array.Empty<Expression<Func<Language, object>>>();
        }
    }
}
