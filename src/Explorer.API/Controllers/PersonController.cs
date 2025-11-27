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

        if (personIdClaim != null && long.TryParse(personIdClaim.Value, out var personId))
        {
            return personId;
        }

        //testovi
        var userIdClaim = HttpContext.User.Claims
            .FirstOrDefault(claim => claim.Type == ClaimTypes.NameIdentifier);

        if (userIdClaim != null && long.TryParse(userIdClaim.Value, out var userId))
        {
            return userId;
        }

        throw new UnauthorizedAccessException("Person ID or User ID not found in token.");
    }

    // POST: api/stakeholders/person
    [Authorize(Policy = "administratorPolicy")]
    [HttpPost]
    public ActionResult<PersonDto> Create([FromBody] AccountRegistrationDto dto)
    {
        var result = _personService.Create(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.UserId }, result);
    }

    // GET: api/stakeholders/person/all
    [Authorize(Policy = "administratorPolicy")]
    [HttpGet("all")]
    public ActionResult<List<PersonDto>> GetAll()
    {
        var result = _personService.GetAll();
        return Ok(result);
    }

    // GET: api/stakeholders/person/{id}
    [Authorize(Policy = "administratorPolicy")]
    [HttpGet("{id:long}")]
    public ActionResult<PersonDto> GetById(long id)
    {
        try
        {
            var result = _personService.Get(id);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }

    // PUT: api/stakeholders/person/{id}/block
    [Authorize(Policy = "administratorPolicy")]
    [HttpPut("{id:long}/block")]
    public ActionResult Block(long id)
    {
        _personService.Block(id);
        return NoContent();
    }

    // PUT: api/stakeholders/person/{id}/unblock
    [Authorize(Policy = "administratorPolicy")]
    [HttpPut("{id:long}/unblock")]
    public ActionResult Unblock(long id)
    {
        _personService.Unblock(id);
        return NoContent();
    }
}