using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist.Preferences;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/preferences")]
[ApiController]
public class PreferenceController : ControllerBase
{
    private readonly IPreferenceService _preferenceService;

    public PreferenceController(IPreferenceService preferenceService)
    {
        _preferenceService = preferenceService;
    }

    [HttpGet]
    public ActionResult<PreferenceDto> GetMyPreferences()
    {
        var touristId = User.PersonId();
        var result = _preferenceService.GetByTouristId(touristId);

        if (result == null)
            return NotFound(); // 404 umesto exception

        return Ok(result);
    }

    [HttpPost]
    public ActionResult<PreferenceDto> Create([FromBody] PreferenceCreateDto preferenceDto)
    {
        var touristId = User.PersonId();
        var result = _preferenceService.Create(preferenceDto, touristId);
        return Ok(result);
    }

    [HttpPut]
    public ActionResult<PreferenceDto> Update([FromBody] PreferenceUpdateDto? preferenceDto)
    {
        var touristId = User.PersonId();

        // Ako je body prazan, vrati trenutne preference
        if (preferenceDto == null)
        {
            var currentPreference = _preferenceService.GetByTouristId(touristId);
            return Ok(currentPreference);
        }

        var result = _preferenceService.Update(preferenceDto, touristId);
        return Ok(result);
    }

    [HttpDelete]
    public ActionResult Delete()
    {
        var touristId = User.PersonId();
        _preferenceService.Delete(touristId);
        return Ok();
    }
}