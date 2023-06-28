using System.Threading;

namespace BandSearch.Database
{
    public interface IRepository<TEntity>
    {
        Task<IReadOnlyCollection<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<TEntity?> FindAsync(int id, CancellationToken cancellationToken);
        Task<TEntity> InsertAsync(TEntity entity, CancellationToken cancellation);
        Task DeleteAsync(int id, CancellationToken cancellationToken);
        Task DeleteAsync(TEntity entityToDelete, CancellationToken cancellationToken);
        Task<TEntity> UpdateAsync(TEntity entityToDelete, CancellationToken cancellationToken);
    }
}
