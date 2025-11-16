using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers;

[Authorize]
[Route("api/stakeholders/person")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IPersonService _personService;

    public PersonController(IPersonService personService)
    {
        _personService = personService;
    }

    [HttpGet]
    public ActionResult<PersonDto> Get()
    {
        try
        {
            var personId = GetPersonIdFromToken();
            var result = _personService.Get(personId);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred: " + ex.Message });
        }
    }

    [HttpPut]
    public ActionResult<PersonDto> Update([FromBody] PersonDto personDto)
    {
        try
        {
            var personId = GetPersonIdFromToken();
            personDto.UserId = personId;
            var result = _personService.Update(personDto);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An unexpected error occurred: " + ex.Message });
        }
    }

    private long GetPersonIdFromToken()
    {
        var personIdClaim = HttpContext.User.Claims
            .FirstOrDefault(claim => claim.Type == "personId");

        if (personIdClaim == null || !long.TryParse(personIdClaim.Value, out var personId))
        {
            throw new UnauthorizedAccessException("Person ID not found in token.");
        }

        return personId;
    }
}