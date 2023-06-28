namespace BandSearch.Web.DTOs
{
    public class BandMemberDTO
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
        public ICollection<InstrumentLevelDTO> InstrumentsLevel { get; set; } = new List<InstrumentLevelDTO>();

        public override bool Equals(object? obj)
        {
            return obj is BandMemberDTO objDTO &&
                   Id == objDTO.Id &&
                   Name == objDTO.Name &&
                   Surname == objDTO.Surname &&
                   Age == objDTO.Age &&
                   Gender == objDTO.Gender &&
                   Country == objDTO.Country &&
                   City == objDTO.City &&
                   PhotoUrl == objDTO.PhotoUrl &&
                   About == objDTO.About &&
                   IsLookingForBand == objDTO.IsLookingForBand &&
                   BandOpenPositionCriteriaInfo == objDTO.BandOpenPositionCriteriaInfo &&
                   InstrumentsLevel == objDTO.InstrumentsLevel;
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
            return hash.ToHashCode();
        }
    }
}
