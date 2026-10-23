
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Underwriting.Application.Interfaces;

namespace Underwriting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RiskScoreController : ControllerBase
    {
        private readonly IRiskScoringService service;

        public RiskScoreController(IRiskScoringService service)
        {
            this.service = service;
        }

        [HttpPost("Calculate/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Calculate(int id)
        {
            var result = await service.CalculateRiskScore(id);

            if (result == null)
                return NotFound("Underwriting case not found.");

            return Ok(result);
        }
    }
}
