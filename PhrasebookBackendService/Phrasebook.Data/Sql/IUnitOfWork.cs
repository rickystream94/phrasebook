using Phrasebook.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Sql
{
    public interface IUnitOfWork
    {
        IBooksRepository BooksRepository { get; }

        IUsersRepository UsersRepository { get; }

        ILanguageRepository LanguageRepository { get; }

        Task ApplyAndSaveChangesAsync(Func<Task> applyChanges);

        Task ApplyAndSaveChangesAsync(Action applyChanges);

        Task SaveChangesAsync();
    }
}
