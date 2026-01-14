using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Authoring;

public interface ISaleService
{
    SaleDto Create(SaleCreateDto saleDto, long authorId);
    SaleDto Update(SaleUpdateDto saleDto, long authorId);
    void Delete(long id, long authorId);
    SaleDto GetById(long id);
    List<SaleDto> GetByAuthorId(long authorId);
}
