using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.BuildingBlocks.Core.UseCases;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Explorer.API.Controllers.Tourist
{
    [Route("api/tourist/facilities")]
    [ApiController]
    public class FacilityTouristController : ControllerBase
    {
        private readonly IFacilityService _facilityService;

        public FacilityTouristController(IFacilityService facilityService)
        {
            _facilityService = facilityService;
        }

        // GET: api/tourist/facilities
        // Turista dobija sve objekte (restoran, WC, itd.) za prikaz na mapi
        [HttpGet]
        [Authorize(Policy = "touristPolicy")]
        public ActionResult<PagedResult<FacilityDto>> GetAll()
        {
            var result = _facilityService.GetAll();
            return Ok(result);
        }
    }
}