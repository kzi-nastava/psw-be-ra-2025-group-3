using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Public.Tourist;
using AutoMapper;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.API.Public.Execution;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class GroupTourSessionService : IGroupTourSessionService
    {
        private readonly IGroupTourSessionRepository _groupTourSessionRepository;        
        private readonly IPositionService _positionService;
        private readonly IGroupTourExecutionService _groupTourExecutionService;

        private readonly IMapper _mapper;

        public GroupTourSessionService(IGroupTourSessionRepository groupTourSessionRespository, IPositionService positionService, IGroupTourExecutionService groupTourExecutionService, IMapper mapper)
        {
            _groupTourSessionRepository = groupTourSessionRespository;
            _positionService = positionService;
            _groupTourExecutionService = groupTourExecutionService;
            _mapper = mapper;
        }

        public GroupTourSessionDto GetSessionById(long sessionId)
        {
            var session = _groupTourSessionRepository.FindById(sessionId);

            return _mapper.Map<GroupTourSessionDto>(session);
        }

        public List<GroupTourSessionDto> GetSessionsByClubId(long clubId)
        {
            var sessions = _groupTourSessionRepository.FindByClubId(clubId);
            if (sessions == null || !sessions.Any())
                throw new KeyNotFoundException("No group tour sessions found for the specified club id");

            var result = new List<GroupTourSessionDto>();

            foreach (var session in sessions)
            {
                var dto = _mapper.Map<GroupTourSessionDto>(session);
                result.Add(dto);
            }

            return result;
        }

        public List<GroupTourSessionDto> GetActiveSessionsByClubId(long clubId)
        {
            var sessions = _groupTourSessionRepository.FindActiveByClubId(clubId);
            if (sessions == null || !sessions.Any())
            {
                return new List<GroupTourSessionDto>();
            }
            var result = new List<GroupTourSessionDto>();

            foreach (var session in sessions)
            {
                var dto = _mapper.Map<GroupTourSessionDto>(session);
                result.Add(dto);
            }

            return result;
        }

        public List<GroupTourSessionDto> GetHighlightedSessionsByClubId(long clubId)
        {
            var sessions = _groupTourSessionRepository.FindHighlightedByClubId(clubId);
            if (sessions == null || !sessions.Any())
            {
                return new List<GroupTourSessionDto>();
            }
            var result = new List<GroupTourSessionDto>();

            foreach (var session in sessions)
            {
                var dto = _mapper.Map<GroupTourSessionDto>(session);
                result.Add(dto);
            }

            return result;
        }

        public List<GroupTourSessionDto> GetSessionsForHighlightMarking(long clubId)
        {
            var sessions = _groupTourSessionRepository.FindSessionsForHighlightMarking(clubId);
            if (sessions == null || !sessions.Any())
            {
                return new List<GroupTourSessionDto>();
            }
            var result = new List<GroupTourSessionDto>();

            foreach (var session in sessions)
            {
                var dto = _mapper.Map<GroupTourSessionDto>(session);
                result.Add(dto);
            }

            return result;
        }

        public List<GroupTourSessionParticipantDto> GetOtherGroupParticipantsByTouristId(long touristId)
        {
            var parList = _groupTourSessionRepository.FindOtherGroupParticipantsByTouristId(touristId);
            
            // Ako nema aktivne group session, vrati praznu listu
            // Turista možda ima solo tour execution, što je validno
            if (parList == null || !parList.Any())
                return new List<GroupTourSessionParticipantDto>();

            var resultList = parList.Select(p =>
            {
                var dto = _mapper.Map<GroupTourSessionParticipantDto>(p);
                dto.Position = _positionService.GetForTourist(dto.TouristId);
                return dto;
            }).ToList();

            return resultList;
        }

        public GroupTourSessionDto GetGroupSessionByTourExecutionId(long tourExecutionId)
        {
            var session = _groupTourSessionRepository.FindById(_groupTourSessionRepository.FindParticipantByTourExecutionId(tourExecutionId).SessionId);

            return _mapper.Map<GroupTourSessionDto>(session);
        }

        public GroupTourSessionDto CreateGroupTourSession(CreateGroupTourSessionDto createDto, long starterId)
        {
            var hasActiveExecution = _groupTourExecutionService.HasActiveExecution(starterId);
            if (hasActiveExecution)
                throw new Exception($"You already have an active tour.");


            var session = new GroupTourSession(
                createDto.TourId,
                createDto.TourName,
                createDto.ClubId,
                starterId);

            var sessionExists = _groupTourSessionRepository
                .FindActiveByClubId(createDto.ClubId)
                .Where(s => s.TourId == createDto.TourId)
                .FirstOrDefault();
            if (sessionExists != null)
                throw new Exception($"An active group tour session for this tour already exists in this club");            

            var createdSession = _groupTourSessionRepository.CreateGroupTourSession(session);
            
            var participant = createdSession.JoinParticipant(
                starterId,
                _groupTourExecutionService.StartGroupExecution(session.TourId, starterId, createdSession.Id));
            var createdParticipant = _groupTourSessionRepository.AddParticipant(participant);

            return _mapper.Map<GroupTourSessionDto>(createdSession);
        }

        public GroupTourSessionDto LeaveGroupTourSession(long sessionId, long touristId)
        {
            var session = _groupTourSessionRepository.FindById(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Group tour session with id {sessionId} not found");


            var participant = session.LeaveParticipant(touristId);
            _groupTourSessionRepository.UpdateParticipant(participant);

            _groupTourExecutionService.LeaveGroupExecution(participant.TouristId);

            return _mapper.Map<GroupTourSessionDto>(_groupTourSessionRepository.Update(session));
        }

        public GroupTourSessionDto JoinGroupTourSession(long sessionId, long touristId)
        {
            var session = _groupTourSessionRepository.FindById(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Group tour session with id {sessionId} not found");

            var participant = session.JoinParticipant(
                touristId,
                _groupTourExecutionService.StartGroupExecution(session.TourId, touristId, session.Id));

            if (participant == null)
                throw new InvalidOperationException($"Tourist with id {touristId} is already a participant of session {sessionId}");

            _groupTourSessionRepository.AddParticipant(participant);
            _groupTourSessionRepository.Update(session);

            return _mapper.Map<GroupTourSessionDto>(session);
        }

        public GroupTourSessionDto HighlightGroupTourSession(long sessionId)
        {
            var session = _groupTourSessionRepository.FindById(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Group tour session not found");
            session.Highlight();
            _groupTourSessionRepository.Update(session);
            return _mapper.Map<GroupTourSessionDto>(session);
        }

        public GroupTourSessionDto RefuseHighlightGroupTourSession(long sessionId)
        {
            var session = _groupTourSessionRepository.FindById(sessionId);
            if (session == null)
                throw new KeyNotFoundException($"Group tour session not found");
            session.RefuseHighlight();
            _groupTourSessionRepository.Update(session);
            return _mapper.Map<GroupTourSessionDto>(session);
        }
    }
}
