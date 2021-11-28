using Microsoft.EntityFrameworkCore;
using Phrasebook.Common.Constants;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Phrasebook.Data.Development
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IUnitOfWork unitOfWork)
        {
            // Creates the DB if it doesn't exist + Apply any pending migration
            unitOfWork.Context.Database.Migrate();

            if (unitOfWork.Context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Add development user
            User developmentUser = await unitOfWork.UserRepository.CreateNewUserAsync(
                Constants.DevelopmentUserIdentityProvider,
                Guid.Parse(Constants.DevelopmentUserPrincipalId),
                Constants.DevelopmentUserEmail,
                Constants.DevelopmentUserFullName,
                Constants.DevelopmentUserFullName);

            IEnumerable<Language> languages = await unitOfWork.LanguageRepository.GetEntitiesAsync();
            Dictionary<string, Language> languagesByCode = languages.ToDictionary(l => l.Id, l => l);

            // Add phrasebooks
            Book it_en = await unitOfWork.BookRepository.CreateNewPhrasebookAsync(languagesByCode["it"], languagesByCode["en"], developmentUser);
            Book it_es = await unitOfWork.BookRepository.CreateNewPhrasebookAsync(languagesByCode["it"], languagesByCode["es"], developmentUser);
            Book en_dk = await unitOfWork.BookRepository.CreateNewPhrasebookAsync(languagesByCode["en"], languagesByCode["dk"], developmentUser);
            Book it_fr = await unitOfWork.BookRepository.CreateNewPhrasebookAsync(languagesByCode["it"], languagesByCode["fr"], developmentUser);

            // Add phrases
            await unitOfWork.PhraseRepository.CreatePhraseAsync(it_en.Id, developmentUser.PrincipalId, "ciao", "hello", LexicalItemType.Interjection, new[] { "hi" });
            await unitOfWork.PhraseRepository.CreatePhraseAsync(it_en.Id, developmentUser.PrincipalId, "mangiare", "to eat", LexicalItemType.Verb);
            await unitOfWork.PhraseRepository.CreatePhraseAsync(it_es.Id, developmentUser.PrincipalId, "ciao", "hola", LexicalItemType.Interjection);
            await unitOfWork.PhraseRepository.CreatePhraseAsync(it_fr.Id, developmentUser.PrincipalId, "ciao", "salut", LexicalItemType.Interjection, new[] { "bonjour" });
        }
    }
}
