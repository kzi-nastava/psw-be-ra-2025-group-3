namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ICouponRepository
{
    Coupon Create(Coupon coupon);
    Coupon Update(Coupon coupon);
    void Delete(long id);
    Coupon? GetById(long id);
    Coupon? GetByCode(string code);
    List<Coupon> GetByAuthorId(long authorId);
    List<Coupon> GetByTourId(long tourId);
}
