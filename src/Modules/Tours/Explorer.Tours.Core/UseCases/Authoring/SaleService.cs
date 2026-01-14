using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Authoring;

public class SaleService : ISaleService
{
    private readonly ISaleRepository _saleRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public SaleService(
        ISaleRepository saleRepository,
        ITourRepository tourRepository,
        IMapper mapper)
    {
        _saleRepository = saleRepository;
        _tourRepository = tourRepository;
        _mapper = mapper;
    }

    public SaleDto Create(SaleCreateDto saleDto, long authorId)
    {
        // Validate that all tours exist and belong to the author
        foreach (var tourId in saleDto.TourIds)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
                throw new NotFoundException($"Tour with id {tourId} not found.");
            if (tour.AuthorId != authorId)
                throw new ForbiddenException($"Tour {tourId} does not belong to you.");
        }

        var sale = new Sale(
            saleDto.TourIds,
            saleDto.StartDate,
            saleDto.EndDate,
            saleDto.DiscountPercentage,
            authorId
        );

        var result = _saleRepository.Create(sale);
        return _mapper.Map<SaleDto>(result);
    }

    public SaleDto Update(SaleUpdateDto saleDto, long authorId)
    {
        var sale = _saleRepository.GetById(saleDto.Id);
        if (sale == null)
            throw new NotFoundException($"Sale with id {saleDto.Id} not found.");
        if (sale.AuthorId != authorId)
            throw new ForbiddenException("You can only update your own sales.");

        // Validate that all tours exist and belong to the author
        foreach (var tourId in saleDto.TourIds)
        {
            var tour = _tourRepository.GetById(tourId);
            if (tour == null)
                throw new NotFoundException($"Tour with id {tourId} not found.");
            if (tour.AuthorId != authorId)
                throw new ForbiddenException($"Tour {tourId} does not belong to you.");
        }

        sale.Update(saleDto.TourIds, saleDto.StartDate, saleDto.EndDate, saleDto.DiscountPercentage);

        var result = _saleRepository.Update(sale);
        return _mapper.Map<SaleDto>(result);
    }

    public void Delete(long id, long authorId)
    {
        var sale = _saleRepository.GetById(id);
        if (sale == null)
            throw new NotFoundException($"Sale with id {id} not found.");
        if (sale.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own sales.");

        _saleRepository.Delete(id);
    }

    public SaleDto GetById(long id)
    {
        var sale = _saleRepository.GetById(id);
        if (sale == null)
            throw new NotFoundException($"Sale with id {id} not found.");

        return _mapper.Map<SaleDto>(sale);
    }

    public List<SaleDto> GetByAuthorId(long authorId)
    {
        var sales = _saleRepository.GetByAuthorId(authorId);
        return sales.Select(_mapper.Map<SaleDto>).ToList();
    }
}
