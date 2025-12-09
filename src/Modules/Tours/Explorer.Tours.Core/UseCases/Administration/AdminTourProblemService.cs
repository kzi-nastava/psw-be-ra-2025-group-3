using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration;

public class AdminTourProblemService : IAdminTourProblemService
{
    private readonly ITourProblemRepository _tourProblemRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public AdminTourProblemService(
        ITourProblemRepository problemRepo, 
        ITourRepository tourRepo, 
        IMapper mapper)
    {
        _tourProblemRepository = problemRepo;
        _tourRepository = tourRepo;
        _mapper = mapper;
    }

    public List<AdminTourProblemDto> GetAll()
    {
        var problems = _tourProblemRepository.GetAll();
        return problems.Select(MapToAdminDto).ToList();
    }

    public AdminTourProblemDto GetById(long id)
    {
        var problem = _tourProblemRepository.GetById(id);
        if (problem == null)
            throw new NotFoundException($"Tour problem with id {id} not found.");
        
        return MapToAdminDto(problem);
    }

    public List<AdminTourProblemDto> GetOverdue(int daysThreshold = 5)
    {
        var problems = _tourProblemRepository.GetOverdue(daysThreshold);
        return problems.Select(MapToAdminDto).ToList();
    }

    private AdminTourProblemDto MapToAdminDto(TourProblem problem)
    {
        var dto = _mapper.Map<AdminTourProblemDto>(problem);
        
        var tour = _tourRepository.GetById(problem.TourId);
        dto.TourName = tour?.Name ?? "Unknown Tour";
        
        dto.IsOverdue = problem.IsOverdue();
        dto.DaysOpen = problem.GetDaysOpen();
        
        return dto;
    }
}
