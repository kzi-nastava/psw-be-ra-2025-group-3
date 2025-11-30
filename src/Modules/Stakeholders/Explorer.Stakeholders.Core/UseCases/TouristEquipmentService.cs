using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.API.Internal;


namespace Explorer.Stakeholders.Core.UseCases;

public class TouristEquipmentService : ITouristEquipmentService
{
    private readonly ITouristRepository _touristRepository;
    private readonly IInternalEquipmentService _equipmentService;

    public TouristEquipmentService(
        ITouristRepository touristRepository,
        IInternalEquipmentService equipmentService)
    {
        _touristRepository = touristRepository;
        _equipmentService = equipmentService;
    }

    public List<EquipmentWithOwnershipDto> GetAllEquipmentWithOwnership(long touristId)
    {
        var tourist = _touristRepository.Get(touristId);
        var allEquipment = _equipmentService.GetAll();

        var ownedIds = tourist?.EquipmentIds ?? new List<long>();

        var result = allEquipment.Select(equipment => new EquipmentWithOwnershipDto
        {
            Id = (int)equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            IsOwnedByTourist = ownedIds.Contains(equipment.Id)
        }).ToList();

        return result;
    }

    public List<EquipmentWithOwnershipDto> GetTouristEquipment(long touristId)
    {
        var tourist = _touristRepository.Get(touristId);

        if (tourist == null || !tourist.EquipmentIds.Any())
            return new List<EquipmentWithOwnershipDto>();

        var allEquipment = _equipmentService.GetAll();

        var result = allEquipment
            .Where(e => tourist.EquipmentIds.Contains(e.Id))
            .Select(equipment => new EquipmentWithOwnershipDto
            {
                Id = (int)equipment.Id,
                Name = equipment.Name,
                Description = equipment.Description,
                IsOwnedByTourist = true
            }).ToList();

        return result;
    }

    public EquipmentWithOwnershipDto AddEquipmentToTourist(long touristId, long equipmentId)
    {
        var equipment = _equipmentService.Get(equipmentId);
        if (equipment == null)
            throw new NotFoundException($"Equipment with id {equipmentId} not found.");

        var tourist = _touristRepository.Get(touristId);

        // Ako turista još ne postoji, kreiraj ga!
        if (tourist == null)
        {
            tourist = new Domain.Tourist(touristId);
            tourist.EquipmentIds.Add(equipmentId);
            _touristRepository.Create(tourist);
        }
        else
        {
            if (tourist.EquipmentIds.Contains(equipmentId))
                throw new InvalidOperationException("Oprema već postoji u vašem spisku.");

            tourist.EquipmentIds.Add(equipmentId);
            _touristRepository.Update(tourist);
        }

        return new EquipmentWithOwnershipDto
        {
            Id = (int)equipment.Id,
            Name = equipment.Name,
            Description = equipment.Description,
            IsOwnedByTourist = true
        };
    }

    public void DeleteEquipmentFromTourist(long touristId, long equipmentId)
    {
        var tourist = _touristRepository.Get(touristId);

        if (tourist == null || !tourist.EquipmentIds.Contains(equipmentId))
            throw new NotFoundException("Oprema nije pronađena u vašem spisku.");

        tourist.EquipmentIds.Remove(equipmentId);
        _touristRepository.Update(tourist);
    }
}