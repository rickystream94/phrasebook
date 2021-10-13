using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Phrasebook.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data
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

            var splitStringConverter = new ValueConverter<IEnumerable<string>, string>(
                list => string.Join(';', list),
                s => s.Split(new[] { ';' }));
            modelBuilder.Entity<Phrase>().Property(nameof(Phrase.ForeignLanguageSynonyms)).HasConversion(splitStringConverter);
        }

        public async Task<T> GetEntityByIdAsync<T>(int? id, params Expression<Func<T, object>>[] navigationPropertiesToInclude)
            where T : EntityBase
        {
            if (!id.HasValue)
            {
                return null;
            }

            // TODO: implement with FindAsync()
            return (await this.GetEntitiesAsync(e => e.Id == id.Value, navigationPropertiesToInclude: navigationPropertiesToInclude)).SingleOrDefault();
        }

        public async Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] navigationPropertiesToInclude)
            where T : EntityBase
        {
            return (await this.GetEntitiesAsync(filter, navigationPropertiesToInclude: navigationPropertiesToInclude)).SingleOrDefault();
        }

        public async Task<IEnumerable<T>> GetEntitiesAsync<T>(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? top = null,
            params Expression<Func<T, object>>[] navigationPropertiesToInclude)
            where T : EntityBase
        {
            IQueryable<T> query = this.Set<T>();

            if (navigationPropertiesToInclude != null)
            {
                foreach (Expression<Func<T, object>> property in navigationPropertiesToInclude)
                {
                    query = query.Include(property);
                }
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (top != null)
            {
                query = query.Take(top.Value);
            }

            return await query.ToArrayAsync();
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Book> Phrasebooks { get; set; }

        public DbSet<Phrase> Phrases { get; set; }

        public DbSet<Language> Languages { get; set; }
    }
}
