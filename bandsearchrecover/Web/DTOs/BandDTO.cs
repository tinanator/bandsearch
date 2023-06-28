namespace BandSearch.Web.DTOs
{
    public class BandDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public ICollection<BandMemberDTO> Members { get; set; } = new List<BandMemberDTO>();
        public ICollection<BandOpenPositionDTO> OpenPositions { get; set; } = new List<BandOpenPositionDTO>();

        public override bool Equals(object? obj)
        {
            return obj is BandDTO dTO &&
                   Id == dTO.Id &&
                   Name == dTO.Name &&
                   OwnerId == dTO.OwnerId &&
                   EqualityComparer<ICollection<BandMemberDTO>>.Default.Equals(Members, dTO.Members) &&
                   EqualityComparer<ICollection<BandOpenPositionDTO>>.Default.Equals(OpenPositions, dTO.OpenPositions);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, OwnerId, Members, OpenPositions);
        }
    }
}
