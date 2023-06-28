using System.Collections.Generic;
using BandSearch.Models;
using BandSearch.Web.BusinessModels;
using Microsoft.AspNetCore.Mvc;

namespace BandSearch.Web.Services
{
    public interface IUserService
    {
        Task<IReadOnlyCollection<UserDetails>> GetListAsync(CancellationToken cancellationToken);
        Task<UserDetails> GetUserDetailsAsync(int userId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<BandOpenPosition>> GetUserBandOpenPositionsAsync(int userId, CancellationToken cancellationToken);
        Task<IReadOnlyCollection<BandView>> GetBandViewsAsync(int iuserId, CancellationToken cancellationToken);
        Task<User> CreateUserAsync(User user, CancellationToken cancellationToken);
        Task DeleteUserAsync(int userId, CancellationToken cancellationToken);
        Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task<User> AddInstrumentLevelAsync(int userId, string instrument, Level level, CancellationToken cancellationToken);
        Task DeleteInstrumentLevelAsync(int userId, int instrumentLevelId, CancellationToken cancellationToken);
    }
}
