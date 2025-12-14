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
    private readonly INotificationRepository _notificationRepository;
    private readonly IMapper _mapper;

    public AdminTourProblemService(
        ITourProblemRepository problemRepo, 
        ITourRepository tourRepo,
        INotificationRepository notificationRepo,
        IMapper mapper)
    {
        _tourProblemRepository = problemRepo;
        _tourRepository = tourRepo;
        _notificationRepository = notificationRepo;
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

        dto.AdminDeadline = problem.AdminDeadline;
        dto.IsDeadlineExpired = problem.IsDeadlineExpired();

        return dto;
    }

    public void SetDeadline(long problemId, DateTime deadline)
    {
        var problem = _tourProblemRepository.GetById(problemId);
        if (problem == null)
            throw new NotFoundException("Problem not found.");

        problem.SetAdminDeadline(deadline);
        _tourProblemRepository.Update(problem);

        var notification = new Notification(
            recipientId: problem.AuthorId,
            type: NotificationType.DeadlineSet,
            relatedEntityId: problem.Id,
            message: $"An administrator has set a deadline for resolving your tour problem. " +
            $"The deadline is {deadline:dd.MM.yyyy} at {deadline:HH:mm}."
        );

        _notificationRepository.Create(notification);
    }

    public void CloseProblem(long problemId)
    {
        var problem = _tourProblemRepository.GetById(problemId);
        if (problem == null)
            throw new NotFoundException("Problem not found.");

        problem.CloseByAdmin();
        _tourProblemRepository.Update(problem);
    }

    public void PenalizeAuthor(long problemId)
    {
        var problem = _tourProblemRepository.GetById(problemId);
        if (problem == null)
            throw new NotFoundException("Problem not found.");

        var tour = _tourRepository.GetById(problem.TourId);
        if (tour == null)
            throw new NotFoundException("Tour not found.");

        tour.Archive(); 
        _tourRepository.Update(tour);
    }
}
