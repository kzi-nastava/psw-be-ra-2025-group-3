using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator.Administration;

[Authorize(Policy = "administratorPolicy")]
[Route("api/administration/accounts")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    // POST: api/administration/accounts
    [HttpPost]
    public ActionResult<AccountDto> Create([FromBody] AccountCreateDto dto)
    {
        var result = _accountService.Create(dto);
        return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
    }

    // GET: api/administration/accounts
    [HttpGet]
    public ActionResult<List<AccountDto>> GetAll()
    {
        var result = _accountService.GetAll();
        return Ok(result);
    }

    // GET: api/administration/accounts/{id}
    [HttpGet("{id:long}")]
    public ActionResult<AccountDto> Get(int id)
    {
        var result = _accountService.Get(id);
        return Ok(result);
    }

    // PUT: api/administration/accounts/{id}/block
    [HttpPut("{id:long}/block")]
    public ActionResult Block(int id)
    {
        _accountService.Block(id);
        return NoContent();
    }

    // PUT: api/administration/accounts/{id}/unblock
    [HttpPut("{id:long}/unblock")]
    public ActionResult Unblock(int id)
    {
        _accountService.Unblock(id);
        return NoContent();
    }
}
