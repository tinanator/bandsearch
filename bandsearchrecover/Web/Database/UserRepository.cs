using System.Collections.Immutable;
using BandSearch.Database;
using BandSearch.Models;
using BandSearch.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BandSearch.Web.Database
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IReadOnlyCollection<Band>> GetBandsAsync(int userId, CancellationToken cancellationToken)
        {
            var bands = (await _context.Users.Include(b => b.Bands)
                .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken))?.Bands ?? new List<Band>().AsReadOnly();

            return bands.ToImmutableList();
        }
        
        public async Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.Email.ToLower() == email.ToLower(), cancellationToken);
        }

        public async override Task<User?> FindAsync(int userId, CancellationToken cancellationToken)
        {
            return await _context.Users
               .Include(u => u.InstrumentsLevel)
               .Include(u => u.Bands)
               .ThenInclude(b => b.OpenPositions)
               .ThenInclude(p => p.InstrumentLevel)
               .FirstOrDefaultAsync(i => i.Id == userId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<BandOpenPosition>> GetOpenPositionsAsync(CancellationToken cancellationToken, params int[] bandIds)
        {
            return await _context.BandOpenPositions.Where(x => bandIds.Contains(x.BandId))
                .ToListAsync(cancellationToken);
        }
    }
}
