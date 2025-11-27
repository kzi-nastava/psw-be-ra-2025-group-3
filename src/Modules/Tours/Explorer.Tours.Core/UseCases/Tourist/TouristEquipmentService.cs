using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristEquipmentService : ITouristEquipmentService
{
    private readonly ITouristEquipmentRepository _touristEquipmentRepository;
    private readonly IEquipmentService _equipmentService;
    private readonly IMapper _mapper;

    public TouristEquipmentService(
        ITouristEquipmentRepository touristEquipmentRepository,
        IEquipmentService equipmentService,
        IMapper mapper)
    {
        _touristEquipmentRepository = touristEquipmentRepository;
        _equipmentService = equipmentService;
        _mapper = mapper;
    }

    public List<EquipmentWithOwnershipDto> GetAllEquipmentWithOwnership(long touristId)
    {
        // 1. Dobij svu opremu koju je admin dodao (preko postojećeg servisa)
        var allEquipment = _equipmentService.GetPaged(0, 1000).Results;

        // 2. Dobij IDs opreme koju turista poseduje
        var touristEquipmentIds = _touristEquipmentRepository.GetByTouristId(touristId)
            .Select(te => te.EquipmentId)
            .ToList();

        // 3. Kreiraj DTO sa oznakom koja oprema je turistina
        var result = allEquipment.Select(equipment => new EquipmentWithOwnershipDto
        {
            Id = (int)equipment.Id,  // cast
            Name = equipment.Name,
            Description = equipment.Description,
            IsOwnedByTourist = touristEquipmentIds.Contains(equipment.Id)
        }).ToList();

        return result;
    }

    public List<EquipmentWithOwnershipDto> GetTouristEquipment(long touristId)
    {
        // 1. Dobij opremu koju turista poseduje
        var touristEquipment = _touristEquipmentRepository.GetByTouristId(touristId);

        // 2. Mapiraj na DTO (sve su IsOwnedByTourist = true)
        var result = touristEquipment.Select(te => new EquipmentWithOwnershipDto
        {
            Id = (int)te.Equipment.Id,  // cast
            Name = te.Equipment.Name,
            Description = te.Equipment.Description,
            IsOwnedByTourist = true
        }).ToList();

        return result;
    }

    public EquipmentWithOwnershipDto AddEquipmentToTourist(TouristEquipmentCreateDto touristEquipmentDto)
    {
        // 1. Provera da li oprema već postoji u turistovom spisku
        var existing = _touristEquipmentRepository.GetByTouristAndEquipment(
            touristEquipmentDto.TouristId,
            touristEquipmentDto.EquipmentId);

        if (existing != null)
        {
            throw new InvalidOperationException("Oprema već postoji u vašem spisku.");
        }

        // 2. Kreiranje novog TouristEquipment entiteta sa validacijom
        var touristEquipment = new TouristEquipment(
            touristEquipmentDto.TouristId,
            touristEquipmentDto.EquipmentId);

        // 3. Čuvanje u bazu
        var result = _touristEquipmentRepository.Create(touristEquipment);

        // 4. Dobijanje Equipment podataka za prikaz
        var equipment = _equipmentService.GetPaged(0, 1000).Results
            .FirstOrDefault(e => e.Id == result.EquipmentId);

        if (equipment == null)
        {
            throw new NotFoundException($"Equipment with id {result.EquipmentId} not found.");
        }

        // 5. Vraćanje DTO sa oznakom
        return new EquipmentWithOwnershipDto
        {
            Id = (int)equipment.Id,  //cast
            Name = equipment.Name,
            Description = equipment.Description,
            IsOwnedByTourist = true
        };
    }

    public void DeleteEquipmentFromTourist(long touristId, long equipmentId)
    {
        // 1. Provera da li oprema postoji u turistovom spisku
        var existing = _touristEquipmentRepository.GetByTouristAndEquipment(touristId, equipmentId);

        if (existing == null)
        {
            throw new NotFoundException("Oprema nije pronađena u vašem spisku.");
        }

        // 2. Brisanje iz baze
        _touristEquipmentRepository.Delete(touristId, equipmentId);
    }
}