using Phrasebook.Common;
using Phrasebook.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Sql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhrasebookDbContext dbContext;
        private readonly Lazy<BooksRepository> booksRepository;
        private readonly Lazy<UsersRepository> usersRepository;
        private readonly Lazy<LanguageRepository> languageRepository;

        public UnitOfWork(PhrasebookDbContext dbContext, ITimeProvider timeProvider)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.booksRepository = new Lazy<BooksRepository>(() => new BooksRepository(this.dbContext, timeProvider));
            this.usersRepository = new Lazy<UsersRepository>(() => new UsersRepository(this.dbContext, timeProvider));
            this.languageRepository = new Lazy<LanguageRepository>(() => new LanguageRepository(this.dbContext));
        }

        public IBooksRepository BooksRepository => this.booksRepository.Value;

        public IUsersRepository UsersRepository => this.usersRepository.Value;

        public ILanguageRepository LanguageRepository => this.languageRepository.Value;

        public async Task ApplyAndSaveChangesAsync(Func<Task> applyChanges)
        {
            if (applyChanges != null)
            {
                await applyChanges();
            }

            await this.SaveChangesAsync();
        }

        public async Task ApplyAndSaveChangesAsync(Action applyChanges)
        {
            await this.ApplyAndSaveChangesAsync(async () =>
            {
                applyChanges?.Invoke();
                await Task.CompletedTask;
            });
        }

        public async Task SaveChangesAsync()
        {
            await this.dbContext.SaveChangesToDatabaseAsync();
        }
    }
}
