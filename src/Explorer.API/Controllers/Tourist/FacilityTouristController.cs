using System.Collections.Generic;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.API.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
        public ActionResult<List<FacilityDto>> GetAll()
        {
            var facilities = _facilityService.GetAll();
            return Ok(facilities);
        }
    }
}
