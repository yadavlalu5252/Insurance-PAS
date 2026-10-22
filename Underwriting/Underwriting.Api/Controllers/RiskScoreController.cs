
using Microsoft.AspNetCore.Mvc;
using Underwriting.Application.Interfaces;

namespace Underwriting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RiskScoreController : ControllerBase
    {
        private readonly IRiskScoringService service;

        public RiskScoreController(IRiskScoringService service)
        {
            this.service = service;
        }

        [HttpPost("Calculate/{id}")]
        public async Task<IActionResult> Calculate(int id)
        {
            var result = await service.CalculateRiskScore(id);

            if (result == null)
                return NotFound("Underwriting case not found.");

            return Ok(result);
        }
    }
}
