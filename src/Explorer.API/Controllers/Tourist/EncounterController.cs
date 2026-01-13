using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/encounters")]
    [ApiController]
    public class EncounterController : ControllerBase
    {
        private readonly IEncounterService _encounterService;

        public EncounterController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
        }

        [HttpGet("active")]
        public ActionResult<List<EncounterDto>> GetActiveEncounters()
        {
            var result = _encounterService.GetActiveEncounters();
            return Ok(result);
        }

        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] EncounterDto encounterDto)
        {
            try
            {
                // Turista NE SME da postavlja status sam (na frontu mu je svakako
                // onemoguceno, ovo je dodatna zastita)
                encounterDto.Status = EncounterStatus.PendingApproval.ToString();

                var result = _encounterService.Create(encounterDto);
                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
        [HttpGet("can-create")]
        public ActionResult<bool> CanCreate()
        {
            // 1) uzmi touristId iz tokena
            var touristId = User.PersonId();

            // 2) pitaj servis
            var canCreate = _encounterService.CanTouristCreateEncounter(touristId);

            return Ok(canCreate);
        }
    }
}