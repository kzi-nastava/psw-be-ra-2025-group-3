using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;

namespace Explorer.Encounters.Core.UseCases;

public class EncounterService : IEncounterService
{
    private readonly IEncounterRepository _encounterRepository;
    private readonly IMapper _mapper;

    public EncounterService(IEncounterRepository encounterRepository, IMapper mapper)
    {
        _encounterRepository = encounterRepository;
        _mapper = mapper;
    }

    public EncounterDto Create(EncounterDto encounterDto)
    {
        var geoPoint = new GeoPoint(encounterDto.Latitude, encounterDto.Longitude);
        var encounterType = Enum.Parse<EncounterType>(encounterDto.Type);

        var encounter = new Encounter(
            encounterDto.Name,
            encounterDto.Description,
            geoPoint,
            encounterDto.XP,
            encounterType
        );

        var result = _encounterRepository.Create(encounter);
        return _mapper.Map<EncounterDto>(result);
    }

    public EncounterDto Update(EncounterDto encounterDto)
    {
        var encounter = _encounterRepository.GetById(encounterDto.Id);
        if (encounter == null)
            throw new KeyNotFoundException($"Encounter with id {encounterDto.Id} not found.");

        var geoPoint = new GeoPoint(encounterDto.Latitude, encounterDto.Longitude);
        var encounterType = Enum.Parse<EncounterType>(encounterDto.Type);
        var encounterStatus = Enum.Parse<EncounterStatus>(encounterDto.Status);

        encounter.Update(
            encounterDto.Name,
            encounterDto.Description,
            geoPoint,
            encounterDto.XP,
            encounterType,
            encounterStatus
        );

        var result = _encounterRepository.Update(encounter);
        return _mapper.Map<EncounterDto>(result);
    }

    public void Delete(long id)
    {
        _encounterRepository.Delete(id);
    }

    public EncounterDto Get(long id)
    {
        var encounter = _encounterRepository.GetById(id);
        if (encounter == null)
            throw new KeyNotFoundException($"Encounter with id {id} not found.");

        return _mapper.Map<EncounterDto>(encounter);
    }

    public List<EncounterDto> GetAll()
    {
        var encounters = _encounterRepository.GetAll();
        return _mapper.Map<List<EncounterDto>>(encounters);
    }

    public List<EncounterDto> GetActiveEncounters()
    {
        var encounters = _encounterRepository.GetActiveEncounters();
        return _mapper.Map<List<EncounterDto>>(encounters);
    }
}