using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases;

public class MeetupService : IMeetupService
{
    private readonly IMeetupRepository _meetupRepository;
    private readonly IMapper _mapper;

    public MeetupService(IMeetupRepository repository, IMapper mapper)
    {
        _meetupRepository = repository;
        _mapper = mapper;
    }

    public List<MeetupDto> GetAll()
    {
        var meetups = _meetupRepository.GetAll();
        return meetups.Select(_mapper.Map<MeetupDto>).ToList();
    }

    public MeetupDto GetById(long id)
    {
        var meetup = _meetupRepository.GetById(id);
        if (meetup == null)
        {
            throw new NotFoundException($"Meetup with ID {id} not found.");
        }
        return _mapper.Map<MeetupDto>(meetup);
    }

    public MeetupDto Create(MeetupCreateDto meetupDto, long creatorId)
    {
        var meetup = new Meetup(
            meetupDto.Title,
            meetupDto.Description,
            meetupDto.DateTime,
            meetupDto.Address,
            meetupDto.Latitude,
            meetupDto.Longitude,
            creatorId,
            meetupDto.TourId
        );

        var createdMeetup = _meetupRepository.Create(meetup);
        return _mapper.Map<MeetupDto>(createdMeetup);
    }

    public MeetupDto Update(long id, MeetupUpdateDto meetupDto, long creatorId)
    {
        var meetup = _meetupRepository.GetById(id);
        if (meetup == null)
        {
            throw new NotFoundException($"Meetup with ID {id} not found.");
        }

        if (meetup.CreatorId != creatorId)
        {
            throw new ForbiddenException("You can only update your own meetups.");
        }

        meetup.Update(
            meetupDto.Title,
            meetupDto.Description,
            meetupDto.DateTime,
            meetupDto.Address,
            meetupDto.Latitude,
            meetupDto.Longitude,
            meetupDto.TourId
        );

        var updatedMeetup = _meetupRepository.Update(meetup);
        return _mapper.Map<MeetupDto>(updatedMeetup);
    }

    public void Delete(long id, long creatorId)
    {
        var meetup = _meetupRepository.GetById(id);
        if (meetup == null)
        {
            throw new NotFoundException($"Meetup with ID {id} not found.");
        }

        if (meetup.CreatorId != creatorId)
        {
            throw new ForbiddenException("You can only delete your own meetups.");
        }

        _meetupRepository.Delete(meetup);
    }

    public List<MeetupDto> GetByTourId(long tourId)
    {
        var meetups = _meetupRepository.GetAll()
                        .Where(m => m.TourId == tourId)
                        .ToList();

        return meetups.Select(m => _mapper.Map<MeetupDto>(m)).ToList();
    }
}
