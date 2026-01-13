using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Internal
{
    public interface IInternalBundleService
    {
        BundleDto GetById(long id);
    }
}