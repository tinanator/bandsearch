using BandSearch.Models;
using BandSearch.Web.Models;

namespace BandSearch.Web.BusinessModels
{
    public class UserDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int? Age { get; set; }
        public int Gender { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? PhotoUrl { get; set; }
        public string? About { get; set; }
        public bool? IsLookingForBand { get; set; }
        public string? BandOpenPositionCriteriaInfo { get; set; }
        public ICollection<InstrumentLevel> InstrumentsLevel { get; set; } = new List<InstrumentLevel>();
        public bool IsBandLookingFor { get; set; }
        public ICollection<BandView> Bands { get; set; } = new List<BandView>();
        public ICollection<BandOpenPosition> BandOpenPositions { get; set; } = new List<BandOpenPosition>();

        public override bool Equals(object? obj)
        {
            return obj is UserDetails details &&
                   Id == details.Id &&
                   Name == details.Name &&
                   Surname == details.Surname &&
                   Age == details.Age &&
                   Gender == details.Gender &&
                   Country == details.Country &&
                   City == details.City &&
                   PhotoUrl == details.PhotoUrl &&
                   About == details.About &&
                   IsLookingForBand == details.IsLookingForBand &&
                   BandOpenPositionCriteriaInfo == details.BandOpenPositionCriteriaInfo &&
                   InstrumentsLevel == details.InstrumentsLevel &&
                   IsBandLookingFor == details.IsBandLookingFor &&
                   Bands.SequenceEqual(details.Bands) &&
                   BandOpenPositions.SequenceEqual(details.BandOpenPositions);
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(Id);
            hash.Add(Name);
            hash.Add(Surname);
            hash.Add(Age);
            hash.Add(Gender);
            hash.Add(Country);
            hash.Add(City);
            hash.Add(PhotoUrl);
            hash.Add(About);
            hash.Add(IsLookingForBand);
            hash.Add(BandOpenPositionCriteriaInfo);
            hash.Add(InstrumentsLevel);
            hash.Add(IsBandLookingFor);
            hash.Add(Bands);
            hash.Add(BandOpenPositions);
            return hash.ToHashCode();
        }
    }
}
