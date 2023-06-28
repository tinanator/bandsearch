using BandSearch.Database;
using BandSearch.Models;

namespace BandSearch.Web.Database
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IReadOnlyCollection<Band>> GetBandsAsync(int userId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<BandOpenPosition>> GetOpenPositionsAsync(CancellationToken cancellationToken, params int[] bandIds);
        Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
