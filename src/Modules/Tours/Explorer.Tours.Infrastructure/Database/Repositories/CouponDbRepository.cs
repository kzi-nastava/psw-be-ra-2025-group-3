using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class CouponDbRepository : ICouponRepository
{
    private readonly ToursContext _context;

    public CouponDbRepository(ToursContext context)
    {
        _context = context;
    }

    public Coupon Create(Coupon coupon)
    {
        _context.Coupons.Add(coupon);
        _context.SaveChanges();
        return coupon;
    }

    public Coupon Update(Coupon coupon)
    {
        try
        {
            _context.Coupons.Update(coupon);
            _context.SaveChanges();
            return coupon;
        }
        catch (DbUpdateException e)
        {
            throw new KeyNotFoundException(e.Message);
        }
    }

    public void Delete(long id)
    {
        var coupon = _context.Coupons.Find(id);
        if (coupon != null)
        {
            _context.Coupons.Remove(coupon);
            _context.SaveChanges();
        }
    }

    public Coupon? GetById(long id)
    {
        return _context.Coupons.Find(id);
    }

    public Coupon? GetByCode(string code)
    {
        return _context.Coupons
            .FirstOrDefault(c => c.Code == code);
    }

    public List<Coupon> GetByAuthorId(long authorId)
    {
        return _context.Coupons
            .Where(c => c.AuthorId == authorId)
            .OrderByDescending(c => c.CreatedAt)
            .ToList();
    }

    public List<Coupon> GetByTourId(long tourId)
    {
        return _context.Coupons
            .Where(c => c.TourId == tourId || c.TourId == null)
            .ToList();
    }
}
