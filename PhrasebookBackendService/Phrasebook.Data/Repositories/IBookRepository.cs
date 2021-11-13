using Phrasebook.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        Task<IEnumerable<Book>> GetPhrasebooksAsync(Guid principalId);

        Task<Book> GetPhrasebookByIdAsync(Guid principalId, int phrasebookId, bool includePhrases = false);

        Task<Book> GetPhrasebookByLanguagesAsync(Guid principalId, Language firstLanguage, Language foreignLanguage, bool includePhrases = false);

        Task<Book> CreateNewPhrasebookAsync(Language firstLanguage, Language foreignLanguage, User user);

        Task<Book> UpdatePhrasebookAsync(Guid principalId, int bookId, Language firstLanguage, Language foreignLanguage);

        Task DeletePhrasebookAsync(Guid principalId, int bookId);
    }
}