using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class KeyPointCompletion : ValueObject
{
    public long KeyPointId { get; }
    public DateTime CompletedAt { get; }

    [JsonConstructor]
    public KeyPointCompletion(long keyPointId, DateTime completedAt)
    {
        if (keyPointId == 0)
            throw new ArgumentException("KeyPoint ID must be valid.", nameof(keyPointId));

        KeyPointId = keyPointId;
        CompletedAt = completedAt;
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return KeyPointId;
        yield return CompletedAt;
    }
}
