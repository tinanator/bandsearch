namespace BandSearch.Web.BusinessModels
{
    public class BandView : IEquatable<BandView>
    {
        public int BandId { get; set; }
        public string BandName { get; set; }
        public List<BandMemberView> Members { get; set; } = new List<BandMemberView>();

        public override bool Equals(object? obj)
        {
            return obj is BandView view &&
                   BandId == view.BandId &&
                   BandName == view.BandName &&
                   Enumerable.SequenceEqual(Members, view.Members);
        }

        public bool Equals(BandView? other)
        {
            if (other is null)
                return false;

            return BandId == other.BandId &&
                   BandName == other.BandName &&
                   Members.SequenceEqual(other.Members);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(BandId, BandName, Members);
        }
    }
}
