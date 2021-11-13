using Phrasebook.Data.Models;
using System;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public interface IPhraseRepository : IGenericRepository<Phrase>
    {
        Task<Phrase> GetPhraseAsync(int bookId, int phraseId, Guid principalId);

        Task<Phrase> UpdatePhraseAsync(int bookId, int phraseId, Guid principalId, string firstLanguagePhrase = null, string foreignLanguagePhrase = null, LexicalItemType? LexicalItemType = null, string[] foreignLanguageSynonyms = null, string description = null);

        Task<Phrase> CreatePhraseAsync(int bookId, Guid principalId, string firstLanguagePhrase, string foreignLanguagePhrase, LexicalItemType LexicalItemType, string[] foreignLanguageSynonyms, string description);

        Task DeletePhraseAsync(int bookId, int phraseId, Guid principalId);
    }
}
