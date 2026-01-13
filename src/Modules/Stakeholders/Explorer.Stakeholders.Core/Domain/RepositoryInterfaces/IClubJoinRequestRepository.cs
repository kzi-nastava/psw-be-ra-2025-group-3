using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IClubJoinRequestRepository
    {
        ClubJoinRequest Get(long id);
        ClubJoinRequest GetByTouristAndClub(long touristId, long clubId);
        ClubJoinRequest Create(ClubJoinRequest request);
        List<ClubJoinRequest> GetByClub(long clubId);
        void Delete(long id);
    }
}