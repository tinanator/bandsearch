using BandSearch.Web.Models;

namespace BandSearch.Models
{
    public record BandOpenPosition
    {
        public int Id { get; set; }
        public int BandId { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public int? Gender { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public virtual InstrumentLevel InstrumentLevel { get; set; }
    }
}
