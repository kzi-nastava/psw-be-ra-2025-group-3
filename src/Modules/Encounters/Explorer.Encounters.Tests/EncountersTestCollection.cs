using Xunit;

namespace Explorer.Encounters.Tests
{
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class EncountersTestCollection : ICollectionFixture<EncountersTestFactory>
    {
        // Ova klasa nema kod, samo označava da svi testovi u ovoj kolekciji
        // dele istu EncountersTestFactory instancu i izvršavaju se SEKVENCIJALNO
    }
}