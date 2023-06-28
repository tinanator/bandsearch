using BandSearch.Models;
using BandSearch.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BandSearch.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext()
        : base()
        { }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options)
        { }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Band> Bands { get; set; }
        public virtual DbSet<InstrumentLevel> InstrumentsLevel { get; set; }
        public virtual DbSet<BandOpenPosition> BandOpenPositions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }
    }
}
