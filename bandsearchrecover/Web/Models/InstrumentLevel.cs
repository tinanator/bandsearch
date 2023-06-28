using BandSearch.Models;

namespace BandSearch.Web.Models
{
    public class InstrumentLevel
    {
        public int Id { get; set; }
        public int? BandOpenPositionId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public User? User { get; set; } = null;
        public BandOpenPosition? BandOpenPosition { get; set; } = null;
        public string Instrument { get; set; }
        public Level Level { get; set; } = 0;
    }
}
