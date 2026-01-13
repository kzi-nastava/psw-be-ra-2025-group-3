using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain
{
    public class ClubJoinRequest : Entity
    {
        public long TouristId { get; private set; }
        public long ClubId { get; private set; }
        public DateTime RequestedAt { get; private set; }

        public ClubJoinRequest() { }

        public ClubJoinRequest(long touristId, long clubId)
        {
            TouristId = touristId;
            ClubId = clubId;
            RequestedAt = DateTime.UtcNow;
            Validate();
        }

        private void Validate()
        {
            if (TouristId == 0) throw new ArgumentException("Invalid TouristId");
            if (ClubId == 0) throw new ArgumentException("Invalid ClubId");
        }
    }
}