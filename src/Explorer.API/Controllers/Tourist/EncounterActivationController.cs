using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.UseCases;
using Explorer.Stakeholders.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Numerics;

namespace Explorer.API.Controllers.Tourist
{
    [Authorize(Policy = "touristPolicy")]
    [Route("api/tourist/encounter-activations")]
    [ApiController]
    public class EncounterActivationController : ControllerBase
    {
        private readonly IEncounterActivationService _encounterActivationService;

        public EncounterActivationController(IEncounterActivationService encounterActivationService)
        {
            _encounterActivationService = encounterActivationService;
        }

        /// Vraća sve encountere u blizini trenutne pozicije turiste
        [HttpGet("nearby")]
        public ActionResult<List<NearbyEncounterDto>> GetNearbyEncounters([FromQuery] double maxDistance = 100)
        {
            try
            {
                long touristId = long.Parse(User.FindFirst("id")!.Value);
                var result = _encounterActivationService.GetNearbyEncounters(touristId, maxDistance);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// Aktivira encounter ako je turista dovoljno blizu
        [HttpPost("{encounterId}/activate")]
        public ActionResult<EncounterActivationDto> ActivateEncounter(long encounterId)
        {
            try
            {
                long touristId = long.Parse(User.FindFirst("id")!.Value);
                var result = _encounterActivationService.ActivateEncounter(touristId, encounterId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// Vraća sve aktivne encountere za trenutnog turistu
        [HttpGet("active")]
        public ActionResult<List<EncounterActivationDto>> GetActiveEncounters()
        {
            long touristId = long.Parse(User.FindFirst("id")!.Value);
            var result = _encounterActivationService.GetActiveEncounters(touristId);
            return Ok(result);
        }

        /// Kompletira aktivni encounter i dodeljuje XP (TODO) <===============================================================
        [HttpPost("{encounterId}/complete")]
        public ActionResult<EncounterActivationDto> CompleteEncounter(long encounterId)
        {
            try
            {
                long touristId = long.Parse(User.FindFirst("id")!.Value);
                var result = _encounterActivationService.CompleteEncounter(touristId, encounterId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Napusta (abandon) aktivni encounter
        [HttpPost("{encounterId}/abandon")]
        public ActionResult<EncounterActivationDto> AbandonEncounter(long encounterId)
        {
            try
            {
                long touristId = long.Parse(User.FindFirst("id")!.Value);
                var result = _encounterActivationService.AbandonEncounter(touristId, encounterId);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}