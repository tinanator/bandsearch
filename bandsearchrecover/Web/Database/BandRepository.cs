using BandSearch.Database;
using BandSearch.Models;
using BandSearch.Web.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace BandSearch.Web.Database
{
    public class BandRepository : Repository<Band>, IBandRepository
    {
        public BandRepository(DatabaseContext context) : base(context)
        {
            _context = context;
        }

        public async override Task<Band?> FindAsync(int bandId, CancellationToken cancellationToken)
        {
            return await _context.Bands
                .Include(b => b.Members)
                .Include(b => b.OpenPositions)
                    .ThenInclude(p => p.InstrumentLevel)
                        .ThenInclude(i => i.User)

                .Include(b => b.Members)
                .Include(b => b.OpenPositions)
                    .ThenInclude(p => p.InstrumentLevel)

                .FirstOrDefaultAsync(i => i.Id == bandId, cancellationToken);
        }

        public async Task<IReadOnlyCollection<BandOpenPosition>> GetOpenPositionsAsync(int bandId, CancellationToken cancellationToken)
        {
            return await _context.BandOpenPositions.Where(s => s.BandId == bandId).ToListAsync(cancellationToken);
        }
    }
}
