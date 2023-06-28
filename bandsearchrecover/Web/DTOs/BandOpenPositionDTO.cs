using BandSearch.Web.Models;

namespace BandSearch.Web.DTOs
{
    public class BandOpenPositionDTO
    {
        public int Id { get; set; }
        public int BandId { get; set; }
        public int? AgeMin { get; set; }
        public int? AgeMax { get; set; }
        public int? Gender { get; set; }
        public InstrumentLevelDTO InstrumentLevel { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }

        public override bool Equals(object? obj)
        {
            return obj is BandOpenPositionDTO dTO &&
                   Id == dTO.Id &&
                   BandId == dTO.BandId &&
                   AgeMin == dTO.AgeMin &&
                   AgeMax == dTO.AgeMax &&
                   Gender == dTO.Gender &&
                   InstrumentLevel == dTO.InstrumentLevel &&
                   Country == dTO.Country &&
                   City == dTO.City;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, BandId, AgeMin, AgeMax, Gender, InstrumentLevel, Country, City);
        }
    }
}
