using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface IPreferenceRepository
{
    Preference Create(Preference preference);
    Preference Update(Preference preference);
    void Delete(long touristId);
    Preference? GetByTouristId(long touristId);
}