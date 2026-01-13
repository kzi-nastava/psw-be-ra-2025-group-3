namespace Explorer.Stakeholders.API.Dtos
{
    public class ClubJoinRequestByTouristDto
    {
        public long Id { get; set; }
        public long ClubId { get; set; }
        public long TouristId { get; set; }
        public string Username { get; set; } 
        public string Email { get; set; }
        public DateTime RequestedAt { get; set; }
    }
}