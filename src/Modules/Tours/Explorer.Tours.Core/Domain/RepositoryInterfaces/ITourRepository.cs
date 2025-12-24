using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITourRepository
{
    Tour Create(Tour tour);
    Tour Update(Tour tour);
    void Delete(long id);
    Tour? GetById(long id);
    Tour? GetWithEquipment(long id);
    List<Tour> GetByAuthorId(long authorId);
    IEnumerable<Tour> GetPublished();
    Tour? GetByIdWithKeyPoints(long id); //za tour-execution

    List<Tour> GetPublishedWithKeyPoints();
    Tour? GetTourWithKeyPoints(long id);

    List<Tour> GetPublishedTours(); // za tour recommendations
}
