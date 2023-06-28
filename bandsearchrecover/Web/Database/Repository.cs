using System.Threading;
using Microsoft.EntityFrameworkCore;

namespace BandSearch.Database
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        internal DatabaseContext _context;
        internal DbSet<TEntity> _dbSet;

        public Repository(DatabaseContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public virtual async Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _dbSet.ToListAsync(cancellationToken);
        }

        public async virtual Task<TEntity?> FindAsync(int id, CancellationToken cancellationToken)
        {
            return await _dbSet.FindAsync(id, cancellationToken);
        }

        public virtual async Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _dbSet.AddAsync(entity, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        public virtual async Task DeleteAsync(int id, CancellationToken cancellationToken)
        {
            TEntity? entityToDelete = await _dbSet.FindAsync(id, cancellationToken);

            if (entityToDelete == null)
            {
                return;
            }

            await DeleteAsync(entityToDelete, cancellationToken);
        }

        public virtual async Task DeleteAsync(TEntity entityToDelete, CancellationToken cancellationToken)
        {
            _dbSet.Remove(entityToDelete);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken)
        {
            _context.Update(entity);
            await _context.SaveChangesAsync(cancellationToken);

            return entity;
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
