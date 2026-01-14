namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ISaleRepository
{
    Sale Create(Sale sale);
    Sale Update(Sale sale);
    void Delete(long id);
    Sale? GetById(long id);
    List<Sale> GetByAuthorId(long authorId);
}
