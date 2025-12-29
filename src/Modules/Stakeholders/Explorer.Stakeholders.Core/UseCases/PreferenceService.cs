using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Core.UseCases;

public class PreferenceService : IPreferenceService
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IMapper _mapper;
    private readonly IInternalTourService _internalTourService;  // tour recommendations

    public PreferenceService(IPreferenceRepository repository, IMapper mapper, IInternalTourService internalTourService)
    {
        _preferenceRepository = repository;
        _mapper = mapper;
        _internalTourService = internalTourService; // za tour recommendations
    }

    public PreferenceDto Create(PreferenceCreateDto preferenceDto, long touristId)
    {
        var existingPreference = _preferenceRepository.GetByTouristId(touristId);
        if (existingPreference != null)
            throw new InvalidOperationException($"Preferences for tourist {touristId} already exist. Use Update instead.");

        var preference = new Preference(
            touristId,
            (TourDifficulty)preferenceDto.Difficulty,
            preferenceDto.WalkingRating,
            preferenceDto.BicycleRating,
            preferenceDto.CarRating,
            preferenceDto.BoatRating,
            preferenceDto.Tags
        );

        var result = _preferenceRepository.Create(preference);
        return _mapper.Map<PreferenceDto>(result);
    }

    public PreferenceDto Update(PreferenceUpdateDto preferenceDto, long touristId)
    {
        var preference = _preferenceRepository.GetByTouristId(touristId);
        if (preference == null)
            throw new NotFoundException($"Preferences for tourist {touristId} not found.");

        if (preference.TouristId != touristId)
            throw new ForbiddenException("You can only update your own preferences.");

        preference.Update(
            (TourDifficulty)preferenceDto.Difficulty,
            preferenceDto.WalkingRating,
            preferenceDto.BicycleRating,
            preferenceDto.CarRating,
            preferenceDto.BoatRating,
            preferenceDto.Tags
        );

        var result = _preferenceRepository.Update(preference);
        return _mapper.Map<PreferenceDto>(result);
    }

    public void Delete(long touristId)
    {
        var preference = _preferenceRepository.GetByTouristId(touristId);
        if (preference == null)
            throw new NotFoundException($"Preferences for tourist {touristId} not found.");

        if (preference.TouristId != touristId)
            throw new ForbiddenException("You can only delete your own preferences.");

        _preferenceRepository.Delete(touristId);
    }

    public PreferenceDto? GetByTouristId(long touristId)
    {
        var preference = _preferenceRepository.GetByTouristId(touristId);

        if (preference == null)
            return null; //  Umesto throw NotFoundException

        return _mapper.Map<PreferenceDto>(preference);
    }


    public List<RecommendedTourDto> GetRecommendedTours(long touristId)
    {
        var preference = _preferenceRepository.GetByTouristId(touristId);
        if (preference == null)
            throw new NotFoundException($"Preferences for tourist {touristId} not found. Please set your preferences first.");

        // Poziv internog API-ja Tours modula
        var allTours = _internalTourService.GetPublishedToursForRecommendation();

        var recommendedTours = new List<RecommendedTourDto>();

        foreach (var tour in allTours)
        {
            int score = CalculateMatchScore(tour, preference);

            if (score > 0)
            {
                recommendedTours.Add(new RecommendedTourDto
                {
                    Id = tour.Id,
                    Name = tour.Name,
                    Description = tour.Description,
                    Difficulty = tour.Difficulty,
                    Price = tour.Price,
                    DistanceInKm = tour.DistanceInKm,
                    Tags = tour.Tags,
                    MatchScore = score
                });
            }
        }

        return recommendedTours.OrderByDescending(t => t.MatchScore).ToList();
    }

    private int CalculateMatchScore(TourForRecommendationDto tour, Preference preference)
    {
        int score = 0;

        // 1. Težina - MORA da odgovara (30 poena)
        if (tour.Difficulty != (int)preference.Difficulty)
            return 0;

        score += 30;

        // 2. Tagovi - bar jedan zajednički tag (40 poena)
        var commonTags = tour.Tags.Intersect(preference.Tags, StringComparer.OrdinalIgnoreCase).Count();
        if (commonTags == 0)
            return 0;

        score += Math.Min(commonTags * 10, 40);

        // 3. Prevoz - proveravamo da li tura ima prevoz koji je turista ocenio >= 2 (30 poena)
        int transportScore = 0;
        foreach (var transportType in tour.TransportationTypes)
        {
            int rating = transportType switch
            {
                0 => preference.WalkingRating,
                1 => preference.BicycleRating,
                2 => preference.CarRating,
                3 => preference.BoatRating,
                _ => 0
            };

            if (rating >= 2)
                transportScore += 10;
        }

        score += Math.Min(transportScore, 30);

        return score;
    }
}