using Explorer.Encounters.API.Dtos;

namespace Explorer.Encounters.API.Public;

public interface IEncounterService
{
    EncounterDto Create(EncounterDto encounterDto);
    EncounterDto Update(EncounterDto encounterDto);
    void Delete(long id);
    EncounterDto Get(long id);
    List<EncounterDto> GetAll();
    List<EncounterDto> GetActiveEncounters();
}