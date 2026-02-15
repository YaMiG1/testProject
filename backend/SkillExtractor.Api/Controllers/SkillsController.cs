using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Api.DTOs;
using SkillExtractor.Api.Services;

namespace SkillExtractor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly ISkillsService _service;

        public SkillsController(ISkillsService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<ActionResult<List<SkillDto>>> GetAll(CancellationToken ct)
        {
            var result = await _service.GetAllAsync(ct);
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<SkillDto>> Create([FromBody] CreateSkillDto req, CancellationToken ct)
        {
            var (ok, result, error) = await _service.CreateAsync(req, ct);

            if (!ok)
            {
                if (error == "duplicate")
                    return Conflict("A skill with the same name already exists.");
                return BadRequest(error);
            }

            return CreatedAtAction(nameof(GetAll), null, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<SkillDto>> Update(int id, [FromBody] UpdateSkillDto req, CancellationToken ct)
        {
            var (ok, result, error, notFound) = await _service.UpdateAsync(id, req, ct);

            if (notFound)
                return NotFound();

            if (!ok)
            {
                if (error == "duplicate")
                    return Conflict("A skill with the same name already exists.");
                return BadRequest(error);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var (ok, notFound) = await _service.DeleteAsync(id, ct);

            if (notFound)
                return NotFound();

            return NoContent();
        }
    }
}
