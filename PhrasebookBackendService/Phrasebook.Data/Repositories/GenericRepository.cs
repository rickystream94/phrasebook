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
        private readonly PhrasebookDbContext context;

        protected GenericRepository(PhrasebookDbContext context)
        {
            this.context = context;
        }

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
            IQueryable<T> query = this.context.Set<T>();

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
            this.context.Set<T>().Add(newEntity);
        }

        public void Delete(T entityToDelete)
        {
            this.context.Set<T>().Remove(entityToDelete);
        }
    }
}
