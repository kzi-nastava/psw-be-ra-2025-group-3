using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces
{
    public interface IPositionRepository
    {
        Position Create(Position position);
        Position Update(Position position);
        Position? GetByTouristId(long touristId);
        bool Exists(long touristId);
    }
}
