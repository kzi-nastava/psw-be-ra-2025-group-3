using System;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Administration;

public interface IAdminTourProblemService
{
    List<AdminTourProblemDto> GetAll();
    AdminTourProblemDto GetById(long id);
    List<AdminTourProblemDto> GetOverdue(int daysThreshold = 5);
    void SetDeadline(long problemId, DateTime deadline);
    void CloseProblem(long problemId);
    void PenalizeAuthor(long problemId);
    AdminTourProblemDto AddAdminMessage(long problemId, long adminId, string content);
}
