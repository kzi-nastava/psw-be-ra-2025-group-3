using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/admin/tour-problems")]
public class AdminTourProblemController : ControllerBase
{
    private readonly IAdminTourProblemService _adminTourProblemService;

    public AdminTourProblemController(IAdminTourProblemService service)
    {
        _adminTourProblemService = service;
    }

    [HttpGet]
    public ActionResult<List<AdminTourProblemDto>> GetAll()
    {
        var result = _adminTourProblemService.GetAll();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<AdminTourProblemDto> GetById(long id)
    {
        var result = _adminTourProblemService.GetById(id);
        return Ok(result);
    }

    [HttpGet("overdue")]
    public ActionResult<List<AdminTourProblemDto>> GetOverdue([FromQuery] int daysThreshold = 5)
    {
        var result = _adminTourProblemService.GetOverdue(daysThreshold);
        return Ok(result);
    }

    [HttpPost("{id}/deadline")]
    public IActionResult SetDeadline(long id, [FromBody] AdminDeadlineDto dto)
    {
        _adminTourProblemService.SetDeadline(id, dto.Deadline);
        return Ok();
    }

    [HttpPost("{id}/close")]
    public IActionResult CloseProblem(long id)
    {
        _adminTourProblemService.CloseProblem(id);
        return Ok();
    }

    [HttpPost("{id}/penalize")]
    public IActionResult Penalize(long id)
    {
        _adminTourProblemService.PenalizeAuthor(id);
        return Ok();
    }
}
