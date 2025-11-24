using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/facilities")]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _facilityService;

    public FacilityController(IFacilityService facilityService)
    {
        _facilityService = facilityService;
    }

    // CREATE
    [HttpPost]
    public ActionResult<FacilityDto> Create([FromBody] FacilityCreateDto dto)
    {
        var result = _facilityService.Create(dto);
        return Ok(result);
    }

    // GET ALL
   [HttpGet]
    public ActionResult<List<FacilityDto>> GetAll()
    {
        var result = _facilityService.GetAll();
        return Ok(result);
    }
   

    // UPDATE
    [HttpPut("{id:long}")]
    public ActionResult<FacilityDto> Update(long id, [FromBody] FacilityUpdateDto dto)
    {
        var result = _facilityService.Update(id, dto);
        return Ok(result);
    }

    // DELETE
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        _facilityService.Delete(id);
        return Ok();
    }
}
