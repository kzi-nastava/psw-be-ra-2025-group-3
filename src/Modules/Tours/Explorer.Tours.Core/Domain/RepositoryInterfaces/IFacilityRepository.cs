using Explorer.Tours.Core.Domain;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IFacilityRepository
{
    Facility Create(Facility facility);
    List<Facility> GetAll();
    Facility? Get(long id);
    Facility Update(Facility facility);
    void Delete(long id);
}
