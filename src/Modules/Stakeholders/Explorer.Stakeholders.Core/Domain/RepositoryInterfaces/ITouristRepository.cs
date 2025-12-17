using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Stakeholders.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

public interface ITouristRepository
{
    Tourist Get(long personId);
    Tourist Create(Tourist tourist);
    Tourist Update(Tourist tourist);
}
