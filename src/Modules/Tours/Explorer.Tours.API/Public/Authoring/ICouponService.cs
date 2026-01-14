using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface ICouponService
{
    CouponDto Create(CouponCreateDto couponDto, long authorId);
    CouponDto Update(long id, CouponUpdateDto couponDto, long authorId);
    void Delete(long id, long authorId);
    CouponDto GetById(long id, long authorId);
    List<CouponDto> GetByAuthorId(long authorId);
    CouponValidationResultDto ValidateCoupon(string code, long tourId);
    CouponValidationResultDto ValidateCouponForCart(string code, List<long> tourIds);
}
