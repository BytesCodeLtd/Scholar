namespace Scholar.Repositories
{
    /// <summary>
    /// Generic data-access contract. Inject <c>IRepository&lt;TEntity&gt;</c> into
    /// controllers/services instead of the DbContext directly.
    /// </summary>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Composable query root. Chain LINQ / EF operators just like a DbSet, e.g.
        /// <c>_repo.Query().Include(x =&gt; x.Children).OrderBy(x =&gt; x.Name).ToListAsync()</c>.
        /// </summary>
        IQueryable<T> Query();

        /// <summary>Finds an entity by primary key.</summary>
        Task<T?> GetByIdAsync(int id);

        Task AddAsync(T entity);

        void Update(T entity);

        void Remove(T entity);

        /// <summary>Persists pending changes. Returns the number of affected rows.</summary>
        Task<int> SaveChangesAsync();
    }
}
