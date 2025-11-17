using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class AwardEventService : IAwardEventService
    {
        private readonly IAwardEventRepository _awardEventRepository;
        private readonly IMapper _mapper;

        public AwardEventService(IAwardEventRepository awardEventRepository, IMapper mapper)
        {
            _awardEventRepository = awardEventRepository;
            _mapper = mapper;
        }

        public PagedResult<AwardEventDto> GetPaged(int page, int pageSize)
        {
            var pagedEvents = _awardEventRepository.GetPaged(page, pageSize);
            return _mapper.Map<PagedResult<AwardEventDto>>(pagedEvents);
        }

        public AwardEventDto Get(long id)
        {
            var awardEvent = _awardEventRepository.Get(id);
            if (awardEvent == null)
            {
                // Baci grešku koju će kontroler uhvatiti kao 404 Not Found
                throw new KeyNotFoundException("Award Event not found: " + id);
            }
            return _mapper.Map<AwardEventDto>(awardEvent);
        }

        public AwardEventDto Create(AwardEventCreateDto awardEventDto)
        {
            // Validacija
            if (awardEventDto.VotingStartDate >= awardEventDto.VotingEndDate)
                throw new ArgumentException("Voting start date must be before end date.");

            if (_awardEventRepository.ExistsForYear(awardEventDto.Year))
                // Baci grešku koju će kontroler uhvatiti kao 409 Conflict
                throw new InvalidOperationException($"An award event for the year {awardEventDto.Year} already exists.");

            try
            {
                var awardEvent = new AwardEvent(
                    awardEventDto.Name,
                    awardEventDto.Description,
                    awardEventDto.Year,
                    awardEventDto.VotingStartDate,
                    awardEventDto.VotingEndDate);

                var createdEvent = _awardEventRepository.Create(awardEvent);
                return _mapper.Map<AwardEventDto>(createdEvent);
            }
            catch (ArgumentException e)
            {
                throw; // 400 Bad Request
            }
        }

        public AwardEventDto Update(AwardEventUpdateDto awardEventDto)
        {
            if (awardEventDto.VotingStartDate >= awardEventDto.VotingEndDate)
                throw new ArgumentException("Voting start date must be before end date.");

            if (_awardEventRepository.ExistsForYear(awardEventDto.Year, awardEventDto.Id))
                throw new InvalidOperationException($"An award event for the year {awardEventDto.Year} already exists.");

            try
            {
                var existingEvent = _awardEventRepository.Get(awardEventDto.Id);
                if (existingEvent == null)
                    throw new KeyNotFoundException("Award Event not found: " + awardEventDto.Id);

                existingEvent.Update(
                    awardEventDto.Name,
                    awardEventDto.Description,
                    awardEventDto.Year,
                    awardEventDto.VotingStartDate,
                    awardEventDto.VotingEndDate);

                var updatedEvent = _awardEventRepository.Update(existingEvent);
                return _mapper.Map<AwardEventDto>(updatedEvent);
            }
            catch (ArgumentException e)
            {
                throw;
            }
        }

        public void Delete(long id)
        {
            var existingEvent = _awardEventRepository.Get(id);
            if (existingEvent == null)
                throw new KeyNotFoundException("Award Event not found: " + id);

            _awardEventRepository.Delete(id);
        }
    }
}