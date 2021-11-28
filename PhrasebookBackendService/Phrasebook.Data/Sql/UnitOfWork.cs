using Phrasebook.Common;
using Phrasebook.Data.Repositories;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Sql
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PhrasebookDbContext dbContext;
        private readonly Lazy<BookRepository> bookRepository;
        private readonly Lazy<UserRepository> userRepository;
        private readonly Lazy<LanguageRepository> languageRepository;
        private readonly Lazy<PhraseRepository> phraseRepository;

        public UnitOfWork(PhrasebookDbContext dbContext, ITimeProvider timeProvider)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.bookRepository = new Lazy<BookRepository>(() => new BookRepository(this.dbContext, timeProvider));
            this.userRepository = new Lazy<UserRepository>(() => new UserRepository(this.dbContext, timeProvider));
            this.languageRepository = new Lazy<LanguageRepository>(() => new LanguageRepository(this.dbContext));
            this.phraseRepository = new Lazy<PhraseRepository>(() => new PhraseRepository(this.dbContext, timeProvider));
        }

        public IBookRepository BookRepository => this.bookRepository.Value;

        public IUserRepository UserRepository => this.userRepository.Value;

        public ILanguageRepository LanguageRepository => this.languageRepository.Value;

        public IPhraseRepository PhraseRepository => this.phraseRepository.Value;

        public PhrasebookDbContext Context => this.dbContext;

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
