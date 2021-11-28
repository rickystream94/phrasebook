using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Phrasebook.Data.Repositories
{
    public interface IGenericRepository<T>
    {
        Task<T> GetEntityAsync(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] navigationPropertiesToInclude);

        Task<IEnumerable<T>> GetEntitiesAsync(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, int? top = null, params Expression<Func<T, object>>[] navigationPropertiesToInclude);

        void Add(T newEntity);

        void Delete(T entityToDelete);
    }
}
