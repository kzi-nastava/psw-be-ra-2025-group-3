namespace Explorer.BuildingBlocks.Core.Domain;

public abstract class Entity
{
    public long Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other || other.GetType() != GetType())
            return false;

        if (Id == 0 && other.Id == 0)
            return ReferenceEquals(this, other);

        return Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id == 0 ? GetType().GetHashCode() : Id.GetHashCode();
    }
}