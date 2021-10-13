using Microsoft.EntityFrameworkCore;
using Phrasebook.Common;
using Phrasebook.Data.Models;
using System;
using System.Linq;

namespace Phrasebook.Data.Development
{
    public static class DbInitializer
    {
        public static void Initialize(PhrasebookDbContext context, ITimeProvider timeProvider)
        {
            // Creates the DB if it doesn't exist + Apply any pending migration
            context.Database.Migrate();

            if (context.Users.Any())
            {
                return; // DB has been seeded
            }

            // Add languages
            Language[] languages = new Language[]
            {
                new Language
                {
                    DisplayName = "English",
                    Code = "en"
                },
                new Language
                {
                    DisplayName = "Italian",
                    Code = "it"
                },
                new Language
                {
                    DisplayName = "Spanish",
                    Code = "sp"
                },
                new Language
                {
                    DisplayName = "French",
                    Code = "fr"
                },
            };
            context.AddRange(languages);
            context.SaveChanges();

            // Add users
            User[] users = new User[]
            {
                new User
                {
                    Email = "test1@test.com",
                    DisplayName = "John Doe",
                    SignedUpOn = timeProvider.Now - TimeSpan.FromDays(100),
                },
                new User
                {
                    Email = "test2@test.com",
                    DisplayName = "Agata Christie",
                    SignedUpOn = timeProvider.Now - TimeSpan.FromDays(200),
                },
                new User
                {
                    Email = "test3@test.com",
                    DisplayName = "Ken Ferry",
                    SignedUpOn = timeProvider.Now - TimeSpan.FromDays(300),
                }
            };
            context.AddRange(users);
            context.SaveChanges();

            // Add phrasebooks
            Book[] phrasebooks = new Book[]
            {
                new Book
                {
                    UserId = 1,
                    FirstLanguageId = 1,
                    ForeignLanguageId = 2,
                    CreatedOn = timeProvider.Now,
                },
                new Book
                {
                    UserId = 2,
                    FirstLanguageId = 2,
                    ForeignLanguageId = 3,
                    CreatedOn = timeProvider.Now - TimeSpan.FromDays(1),
                },
                new Book
                {
                    UserId = 3,
                    FirstLanguageId = 3,
                    ForeignLanguageId = 4,
                    CreatedOn = timeProvider.Now - TimeSpan.FromDays(2),
                }
            };
            context.AddRange(phrasebooks);
            context.SaveChanges();

            // Add phrases
            Phrase[] phrases = new Phrase[]
            {
                new Phrase
                {
                    PhrasebookId = 1,
                    FirstLanguagePhrase = "hi",
                    ForeignLanguagePhrase = "ciao",
                    LexicalItemType = LexicalItemType.Interjection,
                    CreatedOn = timeProvider.Now
                },
                new Phrase
                {
                    PhrasebookId = 1,
                    FirstLanguagePhrase = "to eat",
                    ForeignLanguagePhrase = "mangiare",
                    LexicalItemType = LexicalItemType.Verb,
                    CreatedOn = timeProvider.Now
                },
                new Phrase
                {
                    PhrasebookId = 2,
                    FirstLanguagePhrase = "ciao",
                    ForeignLanguagePhrase = "hola",
                    LexicalItemType = LexicalItemType.Interjection,
                    CreatedOn = timeProvider.Now
                },
                new Phrase
                {
                    PhrasebookId = 3,
                    FirstLanguagePhrase = "hola",
                    ForeignLanguagePhrase = "salut",
                    ForeignLanguageSynonyms = new string[]
                    {
                        "bonjour"
                    },
                    LexicalItemType = LexicalItemType.Interjection,
                    CreatedOn = timeProvider.Now
                },
            };
            context.AddRange(phrases);
            context.SaveChanges();
        }
    }
}
