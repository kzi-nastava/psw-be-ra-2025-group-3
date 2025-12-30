using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Encounters.Core.Domain;

public class EncounterActivation : Entity
{
    public long EncounterId { get; private set; }
    public long TouristId { get; private set; }
    public EncounterActivationStatus Status { get; private set; }
    public DateTime ActivatedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }

    private EncounterActivation() { }

    public EncounterActivation(long encounterId, long touristId)
    {
        EncounterId = encounterId;
        TouristId = touristId;
        Status = EncounterActivationStatus.InProgress;
        ActivatedAt = DateTime.UtcNow;
        CompletedAt = null;
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