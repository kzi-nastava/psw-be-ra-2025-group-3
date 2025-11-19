using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourProblemService : ITourProblemService
{
    private readonly ITourProblemRepository _tourProblemRepository;
    private readonly IMapper _mapper;

    public TourProblemService(ITourProblemRepository repository, IMapper mapper)
    {
        _tourProblemRepository = repository;
        _mapper = mapper;
    }

    public TourProblemDto Create(TourProblemCreateDto problemDto, long touristId)
    {
        // Kreiranje TourProblem entiteta sa validacijama
        var problem = new TourProblem(
            problemDto.TourId,
            touristId,
            (ProblemCategory)problemDto.Category,
            (ProblemPriority)problemDto.Priority,
            problemDto.Description,
            problemDto.Time
        );

        var result = _tourProblemRepository.Create(problem);
        return _mapper.Map<TourProblemDto>(result);
    }

    public TourProblemDto Update(TourProblemUpdateDto problemDto, long touristId)
    {
        // Provera da li problem postoji
        var problem = _tourProblemRepository.GetById(problemDto.Id);
        if (problem == null)
            throw new NotFoundException($"Tour problem with id {problemDto.Id} not found.");

        // Provera da li turista pokušava da izmeni svoj problem
        if (problem.TouristId != touristId)
            throw new ForbiddenException("You can only update your own problems.");

        // Izmena problema kroz domensku metodu
        problem.Update(
            (ProblemCategory)problemDto.Category,
            (ProblemPriority)problemDto.Priority,
            problemDto.Description,
            problemDto.Time
        );

        var result = _tourProblemRepository.Update(problem);
        return _mapper.Map<TourProblemDto>(result);
    }

    public void Delete(long id, long touristId)
    {
        var problem = _tourProblemRepository.GetById(id);
        if (problem == null)
            throw new NotFoundException($"Tour problem with id {id} not found.");

        if (problem.TouristId != touristId)
            throw new ForbiddenException("You can only delete your own problems.");

        _tourProblemRepository.Delete(id);
    }

    public TourProblemDto GetById(long id, long touristId)
    {
        var problem = _tourProblemRepository.GetById(id);
        if (problem == null)
            throw new NotFoundException($"Tour problem with id {id} not found.");

        if (problem.TouristId != touristId)
            throw new ForbiddenException("You can only view your own problems.");

        return _mapper.Map<TourProblemDto>(problem);
    }

    public List<TourProblemDto> GetByTouristId(long touristId)
    {
        var problems = _tourProblemRepository.GetByTouristId(touristId);
        return problems.Select(_mapper.Map<TourProblemDto>).ToList();
    }
}