namespace BandSearch.Web.BusinessModels
{
    public record BandMemberView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}
