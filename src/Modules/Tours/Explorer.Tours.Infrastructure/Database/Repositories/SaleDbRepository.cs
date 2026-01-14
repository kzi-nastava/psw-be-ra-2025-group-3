using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class SaleDbRepository : ISaleRepository
{
    private readonly ToursContext _context;

    public SaleDbRepository(ToursContext context)
    {
        _context = context;
    }

    public Sale Create(Sale sale)
    {
        _context.Sales.Add(sale);
        _context.SaveChanges();
        return sale;
    }

    public Sale Update(Sale sale)
    {
        _context.Sales.Update(sale);
        _context.SaveChanges();
        return sale;
    }

    public void Delete(long id)
    {
        var sale = _context.Sales.Find(id);
        if (sale != null)
        {
            _context.Sales.Remove(sale);
            _context.SaveChanges();
        }
    }

    public Sale? GetById(long id)
    {
        return _context.Sales.Find(id);
    }

    public List<Sale> GetByAuthorId(long authorId)
    {
        return _context.Sales
            .Where(s => s.AuthorId == authorId)
            .OrderByDescending(s => s.CreatedAt)
            .ToList();
    }
}
