using BandSearch.Models;
using BandSearch.Web.BusinessModels;
using BandSearch.Web.Models;

namespace BandSearch.Web.DTOs
{
    public class UserDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public int Age { get; set; }
        public int Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string PhotoUrl { get; set; }
        public string? About { get; set; }
        public bool? IsLookingForBand { get; set; }
        public string? BandOpenPositionCriteriaInfo { get; set; }
        public ICollection<InstrumentLevelDTO>? InstrumentsLevel { get; set; }
        public bool IsBandLookingFor { get; set; }
        public ICollection<BandView> Bands { get; set; } = new List<BandView>();
        public ICollection<BandOpenPositionDTO> BandOpenPositions { get; set; } = new List<BandOpenPositionDTO>();

        public override bool Equals(object? obj)
        {
            return obj is UserDetailsDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   Surname == dTO.Surname &&
                   Age == dTO.Age &&
                   Gender == dTO.Gender &&
                   Country == dTO.Country &&
                   City == dTO.City &&
                   PhotoUrl == dTO.PhotoUrl &&
                   About == dTO.About &&
                   IsLookingForBand == dTO.IsLookingForBand &&
                   BandOpenPositionCriteriaInfo == dTO.BandOpenPositionCriteriaInfo &&
                   InstrumentsLevel == dTO.InstrumentsLevel &&
                   IsBandLookingFor == dTO.IsBandLookingFor &&
                   Bands.SequenceEqual(dTO.Bands) &&
                   BandOpenPositions.SequenceEqual(dTO.BandOpenPositions);
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
