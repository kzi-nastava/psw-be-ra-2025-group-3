using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class EncounterActivation : Entity
{
    public long EncounterId { get; private set; }
    public long TouristId { get; private set; }
    public EncounterActivationStatus Status { get; private set; }
    public DateTime ActivatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    // Za Hidden Location - praćenje vremena na pravoj lokaciji
    public DateTime? LastLocationUpdateAt { get; private set; }
    public double? CurrentLatitude { get; private set; }
    public double? CurrentLongitude { get; private set; }

    // ===== DODAJ OVO =====
    public DateTime? FirstTimeAtCorrectLocationAt { get; private set; }

    private EncounterActivation() { }

    public EncounterActivation(long encounterId, long touristId)
    {
        EncounterId = encounterId;
        TouristId = touristId;
        Status = EncounterActivationStatus.InProgress;
        ActivatedAt = DateTime.UtcNow;
        CompletedAt = null;
        FirstTimeAtCorrectLocationAt = null; // ← DODAJ
    }

    public void UpdateLocation(double latitude, double longitude)
    {
        CurrentLatitude = latitude;
        CurrentLongitude = longitude;
        LastLocationUpdateAt = DateTime.UtcNow;
    }

    // ===== DODAJ OVE DVE METODE =====
    public void MarkFirstTimeAtCorrectLocation()
    {
        if (!FirstTimeAtCorrectLocationAt.HasValue)
        {
            FirstTimeAtCorrectLocationAt = DateTime.UtcNow;
        }
    }

    public void ResetCorrectLocationTimer()
    {
        FirstTimeAtCorrectLocationAt = null;
    }

    // ===== IZMENI OVU METODU =====
    public bool IsAtCorrectLocationForDuration(double targetLat, double targetLon, double maxDistanceMeters, int requiredSeconds)
    {
        // Promeni sa LastLocationUpdateAt na FirstTimeAtCorrectLocationAt!
        if (!CurrentLatitude.HasValue || !CurrentLongitude.HasValue || !FirstTimeAtCorrectLocationAt.HasValue)
            return false;

        var distance = DistanceCalculator.CalculateDistance(
            CurrentLatitude.Value, CurrentLongitude.Value,
            targetLat, targetLon
        );

        if (distance > maxDistanceMeters)
            return false;

        // Računa od PRVOG PUTA kada je stigao na pravo mesto!
        var timeAtLocation = DateTime.UtcNow - FirstTimeAtCorrectLocationAt.Value;
        return timeAtLocation.TotalSeconds >= requiredSeconds;
    }

    public void Complete()
    {
        if (Status == EncounterActivationStatus.Completed)
            throw new InvalidOperationException("Encounter is already completed.");
        Status = EncounterActivationStatus.Completed;
        CompletedAt = DateTime.UtcNow;
    }

    public void Fail()
    {
        if (Status == EncounterActivationStatus.Completed)
            throw new InvalidOperationException("Cannot fail a completed encounter.");
        Status = EncounterActivationStatus.Failed;
        CompletedAt = DateTime.UtcNow;
    }
}