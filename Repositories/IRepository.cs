using System.Linq.Expressions;
using Scholar.Common.Paging;

namespace Scholar.Repositories
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Query();

        Task<T?> GetByIdAsync(int id);

        Task<PagedResult<T>> GetAsync(PageParameters parameters, Expression<Func<T, bool>>? expression = null, params Expression<Func<T, object>>[] includes);

        Task<PagedResult<T>> GetPaginatedByQueryAsync(IQueryable<T> query, PageParameters parameters, Expression<Func<T, bool>>? expression = null, params Expression<Func<T, object>>[] includes);

        Task<PagedResult<TResult>> GetPagedAsync<TResult>(IQueryable<TResult> query, int page, int pageSize);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        Task<int> SaveChangesAsync();
    }
}
