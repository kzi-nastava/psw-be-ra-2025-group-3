using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Administrator.Administration;

[Route("api/administration/equipment")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private readonly IEquipmentService _equipmentService;

    public EquipmentController(IEquipmentService equipmentService)
    {
        _equipmentService = equipmentService;
    }

    [HttpGet]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<PagedResult<EquipmentDto>> GetAll([FromQuery] int page, [FromQuery] int pageSize)
    {
        return Ok(_equipmentService.GetPaged(page, pageSize));
    }

    // DODATO - endpoint koji vraća svu opremu bez paginacije
    [HttpGet("all")]
    [Authorize(Policy = "authorPolicy")]
    public ActionResult<List<EquipmentDto>> GetAllEquipment()
    {
        return Ok(_equipmentService.GetAll());
    }

    [HttpPost]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EquipmentDto> Create([FromBody] EquipmentDto equipment)
    {
        return Ok(_equipmentService.Create(equipment));
    }

    [HttpPut("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult<EquipmentDto> Update([FromBody] EquipmentDto equipment)
    {
        return Ok(_equipmentService.Update(equipment));
    }

    [HttpDelete("{id:long}")]
    [Authorize(Policy = "administratorPolicy")]
    public ActionResult Delete(long id)
    {
        _equipmentService.Delete(id);
        return Ok();
    }
}