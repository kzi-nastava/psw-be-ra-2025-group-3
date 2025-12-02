using System.Security.Claims;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.EquipmentInventory;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/equipment")]
[ApiController]
public class TouristEquipmentController : ControllerBase
{
    private readonly ITouristEquipmentService _touristEquipmentService;

    public TouristEquipmentController(ITouristEquipmentService touristEquipmentService)
    {
        _touristEquipmentService = touristEquipmentService;
    }

    [HttpGet("all")]
    public ActionResult<List<EquipmentWithOwnershipDto>> GetAllEquipmentWithOwnership()
    {
        var touristId = GetTouristId();
        var result = _touristEquipmentService.GetAllEquipmentWithOwnership(touristId);
        return Ok(result);
    }

    [HttpGet("my")]
    public ActionResult<List<EquipmentWithOwnershipDto>> GetMyEquipment()
    {
        var touristId = GetTouristId();
        var result = _touristEquipmentService.GetTouristEquipment(touristId);
        return Ok(result);
    }

    [HttpPost]
    public ActionResult<EquipmentWithOwnershipDto> AddEquipment([FromBody] long equipmentId)
    {
        var touristId = GetTouristId();
        var result = _touristEquipmentService.AddEquipmentToTourist(touristId, equipmentId);
        return Ok(result);
    }

    [HttpDelete("{equipmentId:long}")]
    public ActionResult DeleteEquipment(long equipmentId)
    {
        var touristId = GetTouristId();
        _touristEquipmentService.DeleteEquipmentFromTourist(touristId, equipmentId);
        return NoContent();
    }

    private long GetTouristId()
    {
        var claim = User.Claims.FirstOrDefault(c => c.Type == "personId" || c.Type == ClaimTypes.NameIdentifier);
        if (claim == null || !long.TryParse(claim.Value, out var touristId))
        {
            throw new UnauthorizedAccessException("User is not authenticated or personId claim is missing.");
        }
        return touristId;
    }
}