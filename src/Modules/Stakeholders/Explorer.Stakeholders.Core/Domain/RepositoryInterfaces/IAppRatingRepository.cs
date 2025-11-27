using Explorer.BuildingBlocks.Core.UseCases;
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
        AppRating? GetByUserId(long userId);
        AppRating Update(AppRating rating);
        void Delete(AppRating rating);
        PagedResult<AppRating> GetPaged(int page, int pageSize);
    }
}
