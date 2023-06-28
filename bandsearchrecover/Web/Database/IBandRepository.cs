using BandSearch.Database;
using BandSearch.Models;

namespace BandSearch.Web.Database
{
    public interface IBandRepository : IRepository<Band>
    {
        Task<IReadOnlyCollection<BandOpenPosition>> GetOpenPositionsAsync(int bandId, CancellationToken cancellationToken);
    }
}
