using BandSearch.Models;

namespace BandSearch.Web.DTOs
{
    public class InstrumentLevelDTO
    {
        public int Id { get; set; }
        public int? BandOpenPositionId { get; set; } = null;
        public int? UserId { get; set; } = null;
        public string Instrument { get; set; }
        public Level Level { get; set; } = 0;
    }
}
