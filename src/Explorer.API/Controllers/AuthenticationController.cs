using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers;

[Route("api/users")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;

    public AuthenticationController(IAuthenticationService authenticationService)
    {
        _authenticationService = authenticationService;
    }

    [HttpPost]
    public ActionResult<AuthenticationTokensDto> RegisterTourist([FromBody] AccountRegistrationDto account)
    {
        return Ok(_authenticationService.RegisterTourist(account));
    }

    [HttpPost("login")]
    public ActionResult<AuthenticationTokensDto> Login([FromBody] CredentialsDto credentials)
    {
        return Ok(_authenticationService.Login(credentials));
    }
}