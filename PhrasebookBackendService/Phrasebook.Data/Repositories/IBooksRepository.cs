using Phrasebook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public interface IBooksRepository : IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetUserPhrasebooksAsync(Guid principalId);

        Task<Book> GetUserPhrasebookByIdAsync(Guid principalId, int phrasebookId, params Expression<Func<Book, object>>[] navigationPropertiesToInclude);

        Task<Book> GetUserPhrasebookByLanguagesAsync(Guid principalId, Language firstLanguage, Language foreignLanguage);

        Task<Book> CreateNewPhrasebookAsync(Language firstLanguage, Language foreignLanguage, User user);
    }
}