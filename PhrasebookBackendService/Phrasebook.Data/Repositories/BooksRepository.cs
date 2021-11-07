using Phrasebook.Common;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public class BooksRepository : GenericRepository<Book>, IBooksRepository
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

        public BooksRepository(PhrasebookDbContext context, ITimeProvider timeProvider)
            : base(context)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Book> GetUserPhrasebookByIdAsync(Guid principalId, int phrasebookId, params Expression<Func<Book, object>>[] navigationPropertiesToInclude)
        {
            navigationPropertiesToInclude = navigationPropertiesToInclude == null || navigationPropertiesToInclude.Length == 0
                ? this.extendedProperties
                : navigationPropertiesToInclude;
            return await this.GetEntityAsync(p => p.User.PrincipalId == principalId && p.Id == phrasebookId, navigationPropertiesToInclude);
        }

        public async Task<IEnumerable<Book>> GetUserPhrasebooksAsync(Guid principalId)
        {
            return await this.GetEntitiesAsync(p => p.User.PrincipalId == principalId);
        }

        public async Task<Book> GetUserPhrasebookByLanguagesAsync(Guid principalId, Language firstLanguage, Language foreignLanguage)
        {
            return await this.GetEntityAsync(p => p.FirstLanguage.Id == firstLanguage.Id &&
                    p.ForeignLanguage.Id == foreignLanguage.Id &&
                    p.User.PrincipalId == principalId,
                    this.extendedProperties);
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
            return await this.GetUserPhrasebookByLanguagesAsync(user.PrincipalId, firstLanguage, foreignLanguage);
        }

        protected override Expression<Func<Book, object>>[] GetCommonNavigationProperties()
        {
            return this.properties;
        }
    }
}
