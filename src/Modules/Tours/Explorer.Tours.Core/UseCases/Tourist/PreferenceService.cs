using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class PreferenceService : IPreferenceService
{
    private readonly IPreferenceRepository _preferenceRepository;
    private readonly IMapper _mapper;

    public PreferenceService(IPreferenceRepository repository, IMapper mapper)
    {
        _preferenceRepository = repository;
        _mapper = mapper;
    }

    public PreferenceDto Create(PreferenceCreateDto preferenceDto, long touristId)
    {
        // Provera da li preference vec postoje za ovog turista
        var existingPreference = _preferenceRepository.GetByTouristId(touristId);
        if (existingPreference != null)
            throw new InvalidOperationException($"Preferences for tourist {touristId} already exist. Use Update instead.");

        // Kreiranje Preference entiteta sa validacijama
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
        // Provera da li preference postoje
        var preference = _preferenceRepository.GetByTouristId(touristId);
        if (preference == null)
            throw new NotFoundException($"Preferences for tourist {touristId} not found.");

        // Provera da li turista pokusava da izmeni svoje preference
        if (preference.TouristId != touristId)
            throw new ForbiddenException("You can only update your own preferences.");

        // Izmena preferenci kroz domensku metodu
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

    public PreferenceDto GetByTouristId(long touristId)
    {
        var preference = _preferenceRepository.GetByTouristId(touristId);
        if (preference == null)
            return null;  //  null umesto exception

        return _mapper.Map<PreferenceDto>(preference);
    }
}