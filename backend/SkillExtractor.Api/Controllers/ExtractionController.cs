using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SkillExtractor.Api.DTOs;
using SkillExtractor.Api.Services;

namespace SkillExtractor.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExtractionController : ControllerBase
    {
        private readonly IExtractionService _service;

        public ExtractionController(IExtractionService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        [HttpPost("extract")]
        public async Task<ActionResult<ExtractResponseDto>> Extract([FromBody] ExtractRequestDto dto, CancellationToken ct)
        {
            var (ok, result, error) = await _service.ExtractAndSaveAsync(dto, ct);

            if (!ok)
            {
                if (error == "validation")
                    return BadRequest("Invalid FullName or RawText");
                return StatusCode(500, error);
            }

            return Ok(result);
        }
    }
}
