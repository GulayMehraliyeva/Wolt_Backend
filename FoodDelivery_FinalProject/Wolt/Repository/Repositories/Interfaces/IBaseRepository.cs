using Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Repositories.Interfaces
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
        Task<T> GetByIdAsync(int id, params Expression<Func<T, object>>[] includes);
        Task<T> FindByConditionAsync(
            Expression<Func<T, bool>> predicate,
            params Expression<Func<T, object>>[] includes);

        IQueryable<T> GetQueryable();

        Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate);

        Task<IEnumerable<T>> SortAsync<TKey>(Expression<Func<T, TKey>> keySelector, bool ascending = true);

        Task<int> GetCountAsync();
        Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>> filter,
                                         params Expression<Func<T,object>>[] includes);
    }
}
