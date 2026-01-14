using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Authoring;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Explorer.API.Controllers.Author;

[Authorize(Policy = "authorPolicy")]
[Route("api/sales")]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpPost]
    public ActionResult<SaleDto> Create([FromBody] SaleCreateDto saleDto)
    {
        var authorId = GetAuthorId();
        var result = _saleService.Create(saleDto, authorId);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpGet("my")]
    public ActionResult<List<SaleDto>> GetMySales()
    {
        var authorId = GetAuthorId();
        var result = _saleService.GetByAuthorId(authorId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public ActionResult<SaleDto> GetById(long id)
    {
        var result = _saleService.GetById(id);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public ActionResult<SaleDto> Update(long id, [FromBody] SaleUpdateDto saleDto)
    {
        saleDto.Id = id;
        var authorId = GetAuthorId();
        var result = _saleService.Update(saleDto, authorId);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    public ActionResult Delete(long id)
    {
        var authorId = GetAuthorId();
        _saleService.Delete(id, authorId);
        return NoContent();
    }

    private long GetAuthorId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null || !long.TryParse(claim.Value, out var authorId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or personId claim is missing.");
        }
        return authorId;
    }
}
