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
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeesService _service;

        public EmployeesController(IEmployeesService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpGet]
        public async Task<ActionResult<List<EmployeeListItemDto>>> GetAll(CancellationToken ct)
        {
            var list = await _service.GetAllAsync(ct);
            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeDetailsDto>> GetById(int id, CancellationToken ct)
        {
            var details = await _service.GetByIdAsync(id, ct);
            if (details == null) return NotFound();
            return Ok(details);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var ok = await _service.DeleteAsync(id, ct);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}
