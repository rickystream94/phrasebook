using Phrasebook.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Sql
{
    public interface IUnitOfWork
    {
        IBookRepository BookRepository { get; }

        IUserRepository UserRepository { get; }

        ILanguageRepository LanguageRepository { get; }

        IPhraseRepository PhraseRepository { get; }

        PhrasebookDbContext Context { get; }

        Task ApplyAndSaveChangesAsync(Func<Task> applyChanges);

        Task ApplyAndSaveChangesAsync(Action applyChanges);

        Task SaveChangesAsync();
    }
}
