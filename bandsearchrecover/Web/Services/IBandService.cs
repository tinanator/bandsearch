using System.Threading;
using BandSearch.Models;

namespace BandSearch.Web.Services
{
    public interface IBandService
    {
        Task<Band> CreateBandAsync(string bandName, int userId, CancellationToken cancellationToken);
        Task AddUserToBandAsync(int userId, int bandId, CancellationToken cancellationToken);
        Task DeleteBand(int bandId, CancellationToken cancellationToken);
        Task RemoveUserFromBandAsync(int userId, int bandId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<BandOpenPosition>> GetOpenPositionsAsync(int bandId, CancellationToken cancellationToken);
        Task<BandOpenPosition> CreateOpenPositionAsync(BandOpenPosition bandOpenPosition, CancellationToken cancellationToken);
        Task DeleteOpenPositionAsync(int bandId, int bandOpenPositionId, CancellationToken cancellationToken);
    }
}
