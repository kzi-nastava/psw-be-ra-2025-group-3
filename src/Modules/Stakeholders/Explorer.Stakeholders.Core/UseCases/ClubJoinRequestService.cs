using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class ClubJoinRequestService : IClubJoinRequestService
    {
        private readonly IClubJoinRequestRepository _requestRepository;
        private readonly IClubRepository _clubRepository;
        private readonly IPersonRepository _personRepository;
        private readonly IMapper _mapper;

        public ClubJoinRequestService(IClubJoinRequestRepository requestRepository,
                                      IClubRepository clubRepository,
                                      IPersonRepository personRepository,
                                      IMapper mapper)
        {
            _requestRepository = requestRepository;
            _clubRepository = clubRepository;
            _personRepository = personRepository;
            _mapper = mapper;
        }

        public ClubJoinRequestDto Send(long touristId, long clubId)
        {
            var existingRequest = _requestRepository.GetByTouristAndClub(touristId, clubId);
            if (existingRequest != null)
                throw new ArgumentException("Request already exists.");

            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException("Club not found.");

            if (club.MemberIds.Contains(touristId))
                throw new ArgumentException("Tourist is already a member.");

            var request = new ClubJoinRequest(touristId, clubId);
            _requestRepository.Create(request);

            return _mapper.Map<ClubJoinRequestDto>(request);
        }

        public void Withdraw(long touristId, long requestId)
        {
            var request = _requestRepository.Get(requestId);
            if (request == null)
                throw new KeyNotFoundException("Request not found.");

            if (request.TouristId != touristId)
                throw new UnauthorizedAccessException("You can only withdraw your own requests.");

            _requestRepository.Delete(requestId);
        }

        public void Respond(long ownerId, long requestId, bool accepted)
        {
            var request = _requestRepository.Get(requestId);
            if (request == null)
                throw new KeyNotFoundException("Request not found.");

            var club = _clubRepository.Get(request.ClubId);
            if (club == null)
                throw new KeyNotFoundException("Club not found.");

            if (club.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Only the club owner can respond to requests.");

            if (accepted)
            {
                club.AddMember(request.TouristId);
                _clubRepository.Update(club);
            }

            _requestRepository.Delete(requestId);
        }

        public List<ClubJoinRequestByTouristDto> GetClubJoinRequests(long ownerId, long clubId)
        {
            var club = _clubRepository.Get(clubId);
            if (club == null)
                throw new KeyNotFoundException("Club not found.");

            if (club.OwnerId != ownerId)
                throw new UnauthorizedAccessException("Only owner can see requests.");

            var requests = _requestRepository.GetByClub(clubId);
            var dtos = new List<ClubJoinRequestByTouristDto>();

            foreach (var req in requests)
            {
                var person = _personRepository.Get(req.TouristId);

                dtos.Add(new ClubJoinRequestByTouristDto
                {
                    Id = req.Id,
                    ClubId = req.ClubId,
                    TouristId = req.TouristId,
                    RequestedAt = req.RequestedAt,
                    Username = person != null ? (person.Name + " " + person.Surname) : "Unknown",
                    Email = person != null ? person.Email : "Unknown"
                });
            }

            return dtos;
        }
    }
}