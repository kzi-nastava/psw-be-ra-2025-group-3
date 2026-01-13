using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers.Administrator
{
    [Authorize(Policy = "administratorPolicy")]
    [Route("api/administrator/encounters")]
    [ApiController]
    public class EncounterController : ControllerBase
    {
        private readonly IEncounterService _encounterService;

        public EncounterController(IEncounterService encounterService)
        {
            _encounterService = encounterService;
        }

        [HttpGet]
        public ActionResult<List<EncounterDto>> GetAll()
        {
            var result = _encounterService.GetAll();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public ActionResult<EncounterDto> Get(long id)
        {
            try
            {
                var result = _encounterService.Get(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPost]
        public ActionResult<EncounterDto> Create([FromBody] EncounterDto dto)
        {
            try
            {
                // Kada administrator kreira encounter, on je uvek u draft stanju
                dto.Status = EncounterStatus.Draft.ToString();
                var result = _encounterService.Create(dto);

                return Ok(result);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public ActionResult<EncounterDto> Update(long id, [FromBody] EncounterDto encounterDto)
        {
            try
            {
                encounterDto.Id = id;
                var result = _encounterService.Update(encounterDto);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(long id)
        {
            try
            {
                _encounterService.Delete(id);
                return NoContent();
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
        }

        [HttpPut("{id}/approve")]
        public ActionResult<EncounterDto> Approve(long id)
        {
            try
            {
                var result = _encounterService.Approve(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}/reject")]
        public ActionResult<EncounterDto> Reject(long id)
        {
            try
            {
                var result = _encounterService.Reject(id);
                return Ok(result);
            }
            catch (KeyNotFoundException e)
            {
                return NotFound(e.Message);
            }
            catch (ArgumentException e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}