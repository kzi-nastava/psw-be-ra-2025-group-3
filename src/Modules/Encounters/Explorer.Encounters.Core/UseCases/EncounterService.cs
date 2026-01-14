using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Domain;

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
        var location = new GeoPoint(encounterDto.Latitude, encounterDto.Longitude);
        var status = Enum.Parse<EncounterStatus>(encounterDto.Status);
        var type = Enum.Parse<EncounterType>(encounterDto.Type);

        Encounter encounter;

        if (type == EncounterType.Misc)
        {
            if (string.IsNullOrWhiteSpace(encounterDto.ActionDescription))
                throw new ArgumentException("ActionDescription is required for Misc encounters.");

            encounter = new Encounter(
                encounterDto.Name,
                encounterDto.Description,
                location,
                encounterDto.XP,
                status,
                encounterDto.ActionDescription
            );
        }
        else if (type == EncounterType.Social)
        {
            if (!encounterDto.RequiredPeopleCount.HasValue)
                throw new ArgumentException("RequiredPeopleCount is required for Social encounters.");
            if (!encounterDto.RangeInMeters.HasValue)
                throw new ArgumentException("RangeInMeters is required for Social encounters.");

            encounter = new Encounter(
                encounterDto.Name,
                encounterDto.Description,
                location,
                encounterDto.XP,
                status,
                encounterDto.RequiredPeopleCount.Value,
                encounterDto.RangeInMeters.Value
            );
        }
        else if (type == EncounterType.HiddenLocation)
        {
            if (string.IsNullOrWhiteSpace(encounterDto.ImageUrl))
                throw new ArgumentException("ImageUrl is required for Hidden Location encounters.");

            encounter = new Encounter(
                encounterDto.Name,
                encounterDto.Description,
                location,
                encounterDto.XP,
                status,
                encounterDto.ImageUrl,
                true  // ← bool flag za HiddenLocation
            );
        }
        else
        {
            throw new ArgumentException($"Unknown encounter type: {type}");
        }

        var result = _encounterRepository.Create(encounter);
        return _mapper.Map<EncounterDto>(result);
    }

    public EncounterDto Update(EncounterDto encounterDto)
    {
        var encounter = _encounterRepository.GetById(encounterDto.Id);
        if (encounter == null)
            throw new KeyNotFoundException($"Encounter with id {encounterDto.Id} not found.");

        var location = new GeoPoint(encounterDto.Latitude, encounterDto.Longitude);
        var status = Enum.Parse<EncounterStatus>(encounterDto.Status);
        var type = Enum.Parse<EncounterType>(encounterDto.Type);

        encounter.Update(
            encounterDto.Name,
            encounterDto.Description,
            location,
            encounterDto.XP,
            type,
            status,
            encounterDto.ActionDescription,
            encounterDto.RequiredPeopleCount,
            encounterDto.RangeInMeters,
            encounterDto.ImageUrl
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

    public bool CanTouristCreateEncounter(long touristId)
    {
        // Logika: Proveri da li turist ima XP >= 50 (ili neki drugi uslov)
        // Ako nemaš pristup Tourist servisu, vrati true za sada
        return true;
    }

    public EncounterDto Approve(long encounterId)
    {
        var encounter = _encounterRepository.GetById(encounterId);
        if (encounter == null)
            throw new KeyNotFoundException($"Encounter with id {encounterId} not found.");

        encounter.Activate(); // Postavlja Status na Active
        var result = _encounterRepository.Update(encounter);
        return _mapper.Map<EncounterDto>(result);
    }

    public EncounterDto Reject(long encounterId)
    {
        var encounter = _encounterRepository.GetById(encounterId);
        if (encounter == null)
            throw new KeyNotFoundException($"Encounter with id {encounterId} not found.");

        encounter.Archive(); // Postavlja Status na Archived
        var result = _encounterRepository.Update(encounter);
        return _mapper.Map<EncounterDto>(result);
    }
}