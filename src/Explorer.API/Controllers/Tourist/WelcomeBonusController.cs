using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist;

[Authorize(Policy = "touristPolicy")]
[Route("api/tourist/welcome-bonus")]
[ApiController]
public class WelcomeBonusController : ControllerBase
{
    private readonly IWelcomeBonusService _welcomeBonusService;

    public WelcomeBonusController(IWelcomeBonusService welcomeBonusService)
    {
        _welcomeBonusService = welcomeBonusService;
    }

    [HttpGet]
    public ActionResult<WelcomeBonusDto> GetWelcomeBonus()
    {
        try
        {
            var personId = User.PersonId();
            var bonus = _welcomeBonusService.GetWelcomeBonus(personId);
            return Ok(bonus);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new { message = "Welcome bonus not found." });
        }
    }
}
