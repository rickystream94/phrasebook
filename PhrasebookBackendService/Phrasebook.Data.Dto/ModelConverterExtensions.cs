using Phrasebook.Data.Dto.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Phrasebook.Data.Dto
{
    public static class ModelConverterExtensions
    {
        public static ListResult<T> ToListResult<T>(this IEnumerable<T> self)
        {
            return new ListResult<T>(self.ToArray());
        }

        public static ListResult<T> ToListResult<T>(this T[] self)
        {
            return new ListResult<T>(self);
        }

        public static Phrase ToPhraseDto(this Data.Models.Phrase phrase)
        {
            return new Phrase
            {
                CreatedOn = phrase.CreatedOn,
                CorrectnessCount = phrase.CorrectnessCount,
                Description = phrase.Description,
                FirstLanguagePhrase = phrase.FirstLanguagePhrase,
                ForeignLanguagePhrase = phrase.ForeignLanguagePhrase,
                ForeignLanguageSynonyms = phrase.ForeignLanguageSynonyms != null ? new ReadOnlyCollection<string>(phrase.ForeignLanguageSynonyms.ToList()) : null,
                LexicalItemType = (LexicalItemType)phrase.LexicalItemType,
            };
        }

        public static Book ToPhrasebookDto(this Data.Models.Book book)
        {
            return new Book
            {
                Id = book.Id,
                CreatedOn = book.CreatedOn,
                FirstLanguageDisplayName = book.FirstLanguage.DisplayName,
                ForeignLanguageDisplayName = book.ForeignLanguage.DisplayName,
                UserEmail = book.User.Email,
                Phrases = book.Phrases != null ? new ReadOnlyCollection<Phrase>(book.Phrases.Select(p => p.ToPhraseDto()).ToList()) : null
            };
        }

        public static User ToUserDto(this Data.Models.User user)
        {
            return new User
            {
                Email = user.Email,
                DisplayName = user.DisplayName,
                SignedUpOn = user.SignedUpOn,
                Id = user.Id
            };
        }
    }
}
