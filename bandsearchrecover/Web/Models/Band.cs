namespace BandSearch.Models
{
    public class Band
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OwnerId { get; set; }
        public virtual ICollection<User> Members { get; set; } = new List<User>();
        public virtual ICollection<BandOpenPosition> OpenPositions { get; set; } = new List<BandOpenPosition>();
    }
}
