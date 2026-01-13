using Explorer.Stakeholders.API.Dtos;

namespace Explorer.Stakeholders.API.Public
{
    public interface IClubJoinRequestService
    {
        ClubJoinRequestDto Send(long touristId, long clubId);
        void Withdraw(long touristId, long requestId);
        void Respond(long ownerId, long requestId, bool accepted);
        List<ClubJoinRequestByTouristDto> GetClubJoinRequests(long ownerId, long clubId);
    }
}
