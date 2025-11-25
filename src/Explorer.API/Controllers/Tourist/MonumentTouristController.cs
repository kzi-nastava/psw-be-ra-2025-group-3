using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Route("api/tourist/monuments")]
    [ApiController]
    public class MonumentTouristController : ControllerBase
    {
        private readonly IMonumentService _monumentService;

        public MonumentTouristController(IMonumentService monumentService)
        {
            _monumentService = monumentService;
        }

        
        [HttpGet]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<PagedResult<MonumentDto>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 100)
        {
            var result = _monumentService.GetPaged(page, pageSize);
            return Ok(result);
        }
    }
}
