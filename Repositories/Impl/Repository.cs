using System.Linq.Dynamic.Core.Exceptions;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Scholar.Common.Paging;
using Scholar.Data;
using Scholar.Models;
using static System.Linq.Dynamic.Core.DynamicQueryableExtensions;

namespace Scholar.Repositories.Impl
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ScholarDbContext Context;
        protected readonly DbSet<T> dbSet;

        public Repository(ScholarDbContext context)
        {
            Context = context;
            dbSet = context.Set<T>();
        }

        public IQueryable<T> Query() => dbSet;

        public async Task<T?> GetByIdAsync(int id) => await dbSet.FindAsync(id);

        public Task<PagedResult<T>> GetAsync(
            PageParameters parameters,
            Expression<Func<T, bool>>? expression = null,
            params Expression<Func<T, object>>[] includes)
            => GetPaginatedByQueryAsync(dbSet, parameters, expression, includes);

        public async Task<PagedResult<T>> GetPaginatedByQueryAsync(
            IQueryable<T> query,
            PageParameters parameters,
            Expression<Func<T, bool>>? expression = null,
            params Expression<Func<T, object>>[] includes)
        {
            foreach (Expression<Func<T, object>> include in includes)
            {
                query = query.Include(include);
            }

            int page = parameters.Page < 1 ? 1 : parameters.Page;
            int pageSize = parameters.PageSize < 1 ? 10 : parameters.PageSize;

            int count;
            try
            {
                if (expression != null)
                {
                    query = query.Where(expression);
                }

                if (!string.IsNullOrWhiteSpace(parameters.Filter))
                {
                    query = query.Where(parameters.Filter);
                }

                if (!string.IsNullOrEmpty(parameters.Search))
                {
                    query = query.Where(BuildSearchExpression(parameters.Search));
                }

                if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                {
                    query = query.OrderBy(parameters.OrderBy);
                }

                count = await query.CountAsync();

                query = query.Skip((page - 1) * pageSize)
                             .Take(pageSize);
            }
            catch (ParseException e)
            {
                throw new ArgumentException("Invalid filter/orderBy parameters.", e);
            }

            List<T> items = await query.ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = count
            };
        }

        public Task<PagedResult<TResult>> GetPagedAsync<TResult>(IQueryable<TResult> query, int page, int pageSize)
            => query.ToPagedResultAsync(page, pageSize);

        private static Expression<Func<T, bool>> BuildSearchExpression(string search)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), "e");
            MethodInfo contains = typeof(string).GetMethod(nameof(string.Contains), new[] { typeof(string) })!;
            ConstantExpression term = Expression.Constant(search);
            Expression? body = null;

            foreach (PropertyInfo prop in typeof(T).GetProperties().Where(p => p.PropertyType == typeof(string)))
            {
                MemberExpression member = Expression.Property(param, prop);
                Expression notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                Expression call = Expression.Call(member, contains, term);
                Expression clause = Expression.AndAlso(notNull, call);
                body = body is null ? clause : Expression.OrElse(body, clause);
            }

            body ??= Expression.Constant(true);
            return Expression.Lambda<Func<T, bool>>(body, param);
        }

        public async Task AddAsync(T entity)
        {
            if (entity is IAuditableEntity audit)
            {
                DateTime now = DateTime.UtcNow;
                audit.CreatedAt = now;
                audit.UpdatedAt = now;
                audit.IsActive = true;
            }

            await dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            if (entity is IAuditableEntity audit)
            {
                audit.UpdatedAt = DateTime.UtcNow;
            }

            if (Context.Entry(entity).State == EntityState.Detached)
            {
                dbSet.Update(entity);
            }
        }

        public void Remove(T entity) => dbSet.Remove(entity);

        public Task<bool> SoftDeleteAsync(int id) => SetActiveAsync(id, false);

        public Task<bool> ActivateAsync(int id) => SetActiveAsync(id, true);

        private async Task<bool> SetActiveAsync(int id, bool active)
        {
            T? entity = await dbSet.FindAsync(id);

            if (entity is null)
            {
                return false;
            }

            if (entity is not IAuditableEntity audit)
            {
                throw new NotSupportedException($"{typeof(T).Name} does not implement {nameof(IAuditableEntity)}, so its active state cannot be changed.");
            }

            // Entity is tracked by FindAsync; UpdatedAt is stamped in SaveChangesAsync.
            audit.IsActive = active;
            await SaveChangesAsync();

            return true;
        }

        public async Task<int> SaveChangesAsync()
        {
            StampAuditTimestamps();
            return await Context.SaveChangesAsync();
        }

        // Catches any tracked auditable entities (added or modified) that weren't
        // stamped via AddAsync/Update, so timestamps are always current on save.
        private void StampAuditTimestamps()
        {
            var now = DateTime.UtcNow;
            foreach (var entry in Context.ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedAt = now;
                        entry.Entity.UpdatedAt = now;
                        break;
                    case EntityState.Modified:
                        entry.Entity.UpdatedAt = now;
                        entry.Property(nameof(IAuditableEntity.CreatedAt)).IsModified = false;
                        break;
                }
            }
        }
    }
}
