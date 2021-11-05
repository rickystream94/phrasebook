using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Phrasebook.Data.Models;
using System.Collections.Generic;

namespace Phrasebook.Data.Sql
{
    public class PhrasebookDbContext : DbContext
    {
        public PhrasebookDbContext(DbContextOptions<PhrasebookDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Book>()
                .HasOne(l => l.FirstLanguage)
                .WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull);
            modelBuilder
                .Entity<Book>()
                .HasOne(l => l.ForeignLanguage)
                .WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull);

            modelBuilder
                .Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder
                .Entity<User>()
                .HasIndex(u => u.PrincipalId)
                .IsUnique();

            var splitStringConverter = new ValueConverter<IEnumerable<string>, string>(
                list => string.Join(';', list),
                s => s.Split(new[] { ';' }));
            modelBuilder.Entity<Phrase>().Property(nameof(Phrase.ForeignLanguageSynonyms)).HasConversion(splitStringConverter);
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Book> Phrasebooks { get; set; }

        public DbSet<Phrase> Phrases { get; set; }

        public DbSet<Language> Languages { get; set; }
    }
}
