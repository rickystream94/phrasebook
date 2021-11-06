using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Phrasebook.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Phrasebook.Data.Sql
{
    public class PhrasebookDbContext : DbContext
    {
        public PhrasebookDbContext(DbContextOptions<PhrasebookDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Book> Phrasebooks { get; set; }

        public DbSet<Phrase> Phrases { get; set; }

        public DbSet<Language> Languages { get; set; }

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
            await this.SaveChangesAsync();
        }
    }
}
