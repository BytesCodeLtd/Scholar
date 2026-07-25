using Microsoft.EntityFrameworkCore;
using Scholar.Data;
using Scholar.Models;

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

            dbSet.Update(entity);
        }

        public void Remove(T entity) => dbSet.Remove(entity);

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
