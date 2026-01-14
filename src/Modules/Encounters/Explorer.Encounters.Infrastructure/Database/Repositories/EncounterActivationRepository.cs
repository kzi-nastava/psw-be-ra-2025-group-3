using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Encounters.Infrastructure.Database.Repositories;

public class EncounterActivationRepository : IEncounterActivationRepository
{
    private readonly EncountersContext _dbContext;

    public EncounterActivationRepository(EncountersContext dbContext)
    {
        _dbContext = dbContext;
    }

    public EncounterActivation Create(EncounterActivation activation)
    {
        _dbContext.EncounterActivations.Add(activation);
        _dbContext.SaveChanges();
        return activation;
    }

    public EncounterActivation Update(EncounterActivation activation)
    {
        _dbContext.EncounterActivations.Update(activation);
        _dbContext.SaveChanges();
        return activation;
    }

    public EncounterActivation? GetById(long id)
    {
        return _dbContext.EncounterActivations.Find(id);
    }

    public EncounterActivation? GetActiveByTouristAndEncounter(long touristId, long encounterId)
    {
        return _dbContext.EncounterActivations
            .FirstOrDefault(ea => ea.TouristId == touristId
                               && ea.EncounterId == encounterId
                               && ea.Status == EncounterActivationStatus.InProgress);
    }

    public List<EncounterActivation> GetActiveByTourist(long touristId)
    {
        return _dbContext.EncounterActivations
            .Where(ea => ea.TouristId == touristId && ea.Status == EncounterActivationStatus.InProgress)
            .ToList();
    }

    public List<EncounterActivation> GetCompletedByTourist(long touristId)
    {
        return _dbContext.EncounterActivations
            .Where(ea => ea.TouristId == touristId && ea.Status == EncounterActivationStatus.Completed)
            .ToList();
    }

    public bool HasCompleted(long touristId, long encounterId)
    {
        return _dbContext.EncounterActivations
            .Any(ea => ea.TouristId == touristId
                    && ea.EncounterId == encounterId
                    && ea.Status == EncounterActivationStatus.Completed);
    }

    public List<EncounterActivation> GetActiveActivationsForEncounter(long encounterId)
    {
        return _dbContext.EncounterActivations
            .Where(ea => ea.EncounterId == encounterId && ea.Status == EncounterActivationStatus.InProgress)
            .ToList();
    }

    public List<long> GetActiveTouristIdsInRange(long encounterId, double centerLat, double centerLon, double rangeInMeters)
    {
        var activeActivations = _dbContext.EncounterActivations
            .Where(ea => ea.EncounterId == encounterId && ea.Status == EncounterActivationStatus.InProgress)
            .ToList();

        var touristIdsInRange = new List<long>();

        foreach (var activation in activeActivations)
        {
            if (activation.CurrentLatitude.HasValue && activation.CurrentLongitude.HasValue)
            {
                var distance = DistanceCalculator.CalculateDistance(
                    centerLat, centerLon,
                    activation.CurrentLatitude.Value, activation.CurrentLongitude.Value
                );

                if (distance <= rangeInMeters)
                {
                    touristIdsInRange.Add(activation.TouristId);
                }
            }
        }

        return touristIdsInRange;
    }
}