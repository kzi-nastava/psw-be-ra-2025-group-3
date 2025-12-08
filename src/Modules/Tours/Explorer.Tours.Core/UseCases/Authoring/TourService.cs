using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IEquipmentRepository _equipmentRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository repository, IEquipmentRepository equipmentRepository, IMapper mapper)
    {
        _tourRepository = repository;
        _equipmentRepository = equipmentRepository;
        _mapper = mapper;
    }

    public TourDto Create(TourCreateDto tourDto, long authorId)
    {
        if (authorId == 0) throw new ArgumentException("Author ID must be valid.", nameof(authorId));

        var tour = new Tour(tourDto.Name, tourDto.Description, (TourDifficulty)tourDto.Difficulty, authorId, tourDto.Tags);
        var result = _tourRepository.Create(tour);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Update(TourUpdateDto tourDto, long authorId)
    {
        // Ovde je OK koristiti obican GetById jer Update ne dira opremu direktno
        // (osim ako ne zelis da vratis opremu u responsu, onda stavi GetWithEquipment)
        var tour = _tourRepository.GetById(tourDto.Id);

        if (tour == null) throw new NotFoundException($"Tour with id {tourDto.Id} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("You can only update your own tours.");

        tour.Update(tourDto.Name, tourDto.Description, (TourDifficulty)tourDto.Difficulty, tourDto.Price, tourDto.Tags);

        var durations = _mapper.Map<List<TourDuration>>(tourDto.TourDurations);
        tour.UpdateTourDurations(durations);

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id, long authorId)
    {
        var tour = _tourRepository.GetById(id);
        if (tour == null) throw new NotFoundException($"Tour with id {id} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("You can only delete your own tours.");
        if (tour.Status != TourStatus.Draft) throw new InvalidOperationException("Only tours in Draft status can be deleted.");

        _tourRepository.Delete(id);
    }

    // === IZMENJENO: Koristimo GetWithEquipment ===
    // Ovo je kljucno da bi Edit forma na frontendu dobila listu opreme
    public TourDto GetById(long id)
    {
        var tour = _tourRepository.GetWithEquipment(id);
        if (tour == null) throw new NotFoundException($"Tour with id {id} not found.");
        return _mapper.Map<TourDto>(tour);
    }

    public List<TourDto> GetByAuthorId(long authorId)
    {
        var tours = _tourRepository.GetByAuthorId(authorId);
        return tours.Select(_mapper.Map<TourDto>).ToList();
    }

    public TourDto Publish(long id, long authorId)
    {
        var tour = _tourRepository.GetByIdWithKeyPoints(id);
        if (tour == null) throw new NotFoundException($"Tour with id {id} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("You can only publish your own tours.");

        tour.Publish();
        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Archive(long id, long authorId)
    {
        var tour = _tourRepository.GetById(id);
        if (tour == null) throw new NotFoundException($"Tour with id {id} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("You can only archive your own tours.");

        tour.Archive();
        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Reactivate(long id, long authorId)
    {
        var tour = _tourRepository.GetById(id);
        if (tour == null) throw new NotFoundException($"Tour with id {id} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("You can only reactivate your own tours.");

        tour.Reactivate();
        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    // === IZMENJENO: Koristimo GetWithEquipment ===
    public TourDto AddEquipment(long tourId, long equipmentId, long authorId)
    {
        // Moramo ucitati opremu da bi domenska logika (Contains) radila ispravno
        var tour = _tourRepository.GetWithEquipment(tourId);
        if (tour == null) throw new NotFoundException($"Tour with ID {tourId} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("Only the author can add equipment.");

        var equipment = _equipmentRepository.Get(equipmentId);
        if (equipment == null) throw new NotFoundException($"Equipment with ID {equipmentId} not found.");

        tour.AddEquipment(equipment);

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    // === IZMENJENO: Koristimo GetWithEquipment ===
    public TourDto RemoveEquipment(long tourId, long equipmentId, long authorId)
    {
        // Isto kao za Add, moramo znati sta je unutra
        var tour = _tourRepository.GetWithEquipment(tourId);
        if (tour == null) throw new NotFoundException($"Tour with ID {tourId} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("Only the author can remove equipment.");

        var equipment = _equipmentRepository.Get(equipmentId);
        if (equipment == null) throw new NotFoundException($"Equipment with ID {equipmentId} not found.");

        tour.RemoveEquipment(equipment);

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }

    public List<TourDto> GetPublished()
    {
        var tours = _tourRepository.GetPublished();
        return _mapper.Map<List<TourDto>>(tours);
    }

    // Azuriranje duzine ture (u km)
    public TourDto UpdateDistance(long tourId, double distanceInKm, long authorId)
    {
        var tour = _tourRepository.GetById(tourId);
        if (tour == null) throw new NotFoundException($"Tour with id {tourId} not found.");
        if (tour.AuthorId != authorId) throw new ForbiddenException("You can only update your own tours.");

        tour.UpdateDistance(distanceInKm);

        var result = _tourRepository.Update(tour);
        return _mapper.Map<TourDto>(result);
    }
}