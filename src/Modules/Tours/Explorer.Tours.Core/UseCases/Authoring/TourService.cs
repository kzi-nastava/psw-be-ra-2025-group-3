using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository repository, IMapper mapper)
    {
        _tourRepository = repository;
        _mapper = mapper;
    }

    public TourDto Create(TourCreateDto tourDto, long authorId)
    {
        // Basic validation - full Stakeholders integration can be added later !!!!!!!!!!!!!!!!!!!!!!!!!!!!
        if (authorId == 0)
            throw new ArgumentException("Author ID must be valid.", nameof(authorId));

        var tour = new Tour(
            tourDto.Name,
            tourDto.Description,
            (TourDifficulty)tourDto.Difficulty,
            authorId,
            tourDto.Tags
        );

        var result = _tourRepository.Create(tour);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Update(TourUpdateDto tourDto, long authorId)
    {
        // Provera da li tura postoji
        var tour = _tourRepository.GetById(tourDto.Id);
        if (tour == null)
            throw new NotFoundException($"Tour with id {tourDto.Id} not found.");

        // Provera da li autor pokusava da izmeni svoju turu
        if (tour.AuthorId != authorId)
            throw new ForbiddenException("You can only update your own tours.");

        // Izmena ture kroz domensku metodu
        tour.Update(
            tourDto.Name,
            tourDto.Description,
            (TourDifficulty)tourDto.Difficulty,
            tourDto.Price,
            tourDto.Tags
        );

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id, long authorId)
    {
        var tour = _tourRepository.GetById(id);
        if (tour == null)
            throw new NotFoundException($"Tour with id {id} not found.");

        if (tour.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own tours.");

        // Provera da li je tura u Draft statusu
        if (tour.Status != TourStatus.Draft)
            throw new InvalidOperationException("Only tours in Draft status can be deleted.");

        _tourRepository.Delete(id);
    }

    public TourDto GetById(long id)
    {
        var tour = _tourRepository.GetById(id);
        if (tour == null)
            throw new NotFoundException($"Tour with id {id} not found.");

        return _mapper.Map<TourDto>(tour);
    }

    public List<TourDto> GetByAuthorId(long authorId)
    {
        var tours = _tourRepository.GetByAuthorId(authorId);
        return tours.Select(_mapper.Map<TourDto>).ToList();
    }

    public TourDto Publish(long id, long authorId)
    {
        var tour = _tourRepository.GetById(id);
        if (tour == null)
            throw new NotFoundException($"Tour with id {id} not found.");

        if (tour.AuthorId != authorId)
            throw new ForbiddenException("You can only publish your own tours.");

        // Objavljivanje kroz domensku metodu
        tour.Publish();

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }
}