using Phrasebook.Common;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public class PhraseRepository : GenericRepository<Phrase>, IPhraseRepository
    {
        private readonly Expression<Func<Phrase, object>>[] properties = new Expression<Func<Phrase, object>>[]
        {
            p => p.Phrasebook,
            p => p.Phrasebook.User,
        };
        private readonly ITimeProvider timeProvider;

        public PhraseRepository(PhrasebookDbContext context, ITimeProvider timeProvider)
            : base(context)
        {
            this.timeProvider = timeProvider ?? throw new ArgumentNullException(nameof(timeProvider));
        }

        public async Task<Phrase> GetPhraseAsync(int bookId, int phraseId, Guid principalId)
        {
            return await this.GetEntityAsync(p => p.Id == phraseId && p.PhrasebookId == bookId && p.Phrasebook.User.PrincipalId == principalId);
        }

        public async Task<Phrase> CreatePhraseAsync(int bookId, Guid principalId, string firstLanguagePhrase, string foreignLanguagePhrase, LexicalItemType lexicalItemType, string[] foreignLanguageSynonyms, string description)
        {
            Phrase phrase = new Phrase
            {
                FirstLanguagePhrase = firstLanguagePhrase,
                ForeignLanguagePhrase = foreignLanguagePhrase,
                ForeignLanguageSynonyms = foreignLanguageSynonyms,
                LexicalItemType = lexicalItemType,
                PhrasebookId = bookId,
                CreatedOn = this.timeProvider.Now,
                Description = description
            };
            await this.Context.ApplyAndSaveChangesAsync(() => this.Add(phrase));

            // Reload from DB, so we get the updated properties including the ID
            await this.Context.ReloadEntityAsync(phrase);

            return phrase;
        }

        public async Task<Phrase> UpdatePhraseAsync(int bookId, int phraseId, Guid principalId, string firstLanguagePhrase = null, string foreignLanguagePhrase = null, LexicalItemType? lexicalItemType = null, string[] foreignLanguageSynonyms = null, string description = null)
        {
            Phrase phraseToUpdate = await this.GetPhraseAsync(bookId, phraseId, principalId);
            await this.Context.ApplyAndSaveChangesAsync(() =>
            {
                if (!string.IsNullOrWhiteSpace(firstLanguagePhrase))
                {
                    phraseToUpdate.FirstLanguagePhrase = firstLanguagePhrase;
                }

                if (!string.IsNullOrWhiteSpace(foreignLanguagePhrase))
                {
                    phraseToUpdate.ForeignLanguagePhrase = foreignLanguagePhrase;
                }

                if (lexicalItemType.HasValue)
                {
                    phraseToUpdate.LexicalItemType = lexicalItemType.Value;
                }

                if (foreignLanguageSynonyms != null)
                {
                    phraseToUpdate.ForeignLanguageSynonyms = foreignLanguageSynonyms;
                }

                if (description != null)
                {
                    phraseToUpdate.Description = description;
                }
            });

            await this.Context.ReloadEntityAsync(phraseToUpdate);
            return phraseToUpdate;
        }

        public async Task DeletePhraseAsync(int bookId, int phraseId, Guid principalId)
        {
            Phrase phraseToDelete = await this.GetPhraseAsync(bookId, phraseId, principalId);
            await this.Context.ApplyAndSaveChangesAsync(() => this.Delete(phraseToDelete));
        }

        protected override Expression<Func<Phrase, object>>[] GetCommonNavigationProperties()
        {
            return this.properties;
        }
    }
}
