using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain.RepositoryInterfaces
{
    public interface IAppRatingRepository
    {
        AppRating Create(AppRating rating);
        List<AppRating> GetAll();
        AppRating? GetByUserId(long userId);
        AppRating Update(AppRating rating);
        void Delete(AppRating rating);
    }
}
