using Phrasebook.Common;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly Expression<Func<Book, object>>[] properties = new Expression<Func<Book, object>>[]
        {
            b => b.FirstLanguage,
            b => b.ForeignLanguage,
            b => b.User
        };

        private readonly Expression<Func<Book, object>>[] extendedProperties = new Expression<Func<Book, object>>[]
        {
            b => b.FirstLanguage,
            b => b.ForeignLanguage,
            b => b.User,
            b => b.Phrases,
        };

        private readonly ITimeProvider timeProvider;

        public BookRepository(PhrasebookDbContext context, ITimeProvider timeProvider)
            : base(context)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Book> GetPhrasebookByIdAsync(Guid principalId, int phrasebookId, bool includePhrases = false)
        {
            Expression<Func<Book, object>>[] navigationPropertiesToInclude = includePhrases
                ? this.extendedProperties
                : this.properties;
            return await this.GetEntityAsync(p => p.User.PrincipalId == principalId && p.Id == phrasebookId, navigationPropertiesToInclude);
        }

        public async Task<IEnumerable<Book>> GetPhrasebooksAsync(Guid principalId)
        {
            return await this.GetEntitiesAsync(p => p.User.PrincipalId == principalId);
        }

        public async Task<Book> GetPhrasebookByLanguagesAsync(Guid principalId, Language firstLanguage, Language foreignLanguage, bool includePhrases = false)
        {
            return await this.GetEntityAsync(p => p.FirstLanguage.Id == firstLanguage.Id &&
                    p.ForeignLanguage.Id == foreignLanguage.Id &&
                    p.User.PrincipalId == principalId,
                    includePhrases ? this.extendedProperties : this.properties);
        }

        public async Task<Book> CreateNewPhrasebookAsync(Language firstLanguage, Language foreignLanguage, User user)
        {
            Book bookToCreate = new Book
            {
                CreatedOn = this.timeProvider.Now,
                FirstLanguageId = firstLanguage.Id,
                ForeignLanguageId = foreignLanguage.Id,
                UserId = user.Id,
            };
            await this.Context.ApplyAndSaveChangesAsync(() => this.Add(bookToCreate));

            // Get newly created phrasebook from the DB
            await this.Context.ReloadEntityAsync(bookToCreate);
            return bookToCreate;
        }

        public async Task<Book> UpdatePhrasebookAsync(Guid principalId, int bookId, Language firstLanguage, Language foreignLanguage)
        {
            Book bookToUpdate = await this.GetPhrasebookByIdAsync(principalId, bookId);

            await this.Context.ApplyAndSaveChangesAsync(() =>
            {
                bookToUpdate.FirstLanguageId = firstLanguage.Id;
                bookToUpdate.ForeignLanguageId = foreignLanguage.Id;
            });

            // Reload phrasebook from DB (so it includes the updated navigation properties)
            await this.Context.ReloadEntityAsync(bookToUpdate);
            return bookToUpdate;
        }

        public async Task DeletePhrasebookAsync(Guid principalId, int bookId)
        {
            Book existingBook = await this.GetPhrasebookByIdAsync(principalId, bookId);
            await this.Context.ApplyAndSaveChangesAsync(() => this.Delete(existingBook));
        }

        protected override Expression<Func<Book, object>>[] GetCommonNavigationProperties()
        {
            return this.properties;
        }
    }
}
