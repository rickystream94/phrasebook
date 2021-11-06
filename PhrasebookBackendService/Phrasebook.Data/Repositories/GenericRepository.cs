using Microsoft.EntityFrameworkCore;
using Phrasebook.Data.Models;
using Phrasebook.Data.Sql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : EntityBase
    {
        protected GenericRepository(PhrasebookDbContext context)
        {
            this.Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected PhrasebookDbContext Context { get; }

        public async Task<T> GetEntityByIdAsync(int? id, params Expression<Func<T, object>>[] navigationPropertiesToInclude)
        {
            if (!id.HasValue)
            {
                return null;
            }

            // TODO: implement with FindAsync()
            return (await this.GetEntitiesAsync(e => e.Id == id.Value, navigationPropertiesToInclude: navigationPropertiesToInclude)).SingleOrDefault();
        }

        public async Task<T> GetEntityAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] navigationPropertiesToInclude)
        {
            return (await this.GetEntitiesAsync(filter, navigationPropertiesToInclude: navigationPropertiesToInclude)).SingleOrDefault();
        }

        public async Task<IEnumerable<T>> GetEntitiesAsync(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            int? top = null,
            params Expression<Func<T, object>>[] navigationPropertiesToInclude)
        {
            IQueryable<T> query = this.Context.Set<T>();

            if (navigationPropertiesToInclude == null)
            {
                navigationPropertiesToInclude = this.GetCommonNavigationProperties();
            }

            foreach (Expression<Func<T, object>> property in navigationPropertiesToInclude)
            {
                query = query.Include(property);
            }

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (top != null)
            {
                query = query.Take(top.Value);
            }

            return await query.ToArrayAsync();
        }

        public void Add(T newEntity)
        {
            this.Context.Set<T>().Add(newEntity);
        }

        public void Delete(T entityToDelete)
        {
            this.Context.Set<T>().Remove(entityToDelete);
        }

        protected abstract Expression<Func<T, object>>[] GetCommonNavigationProperties();
    }
}
