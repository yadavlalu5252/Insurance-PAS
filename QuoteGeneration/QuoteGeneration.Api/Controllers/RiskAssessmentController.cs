
using Microsoft.AspNetCore.Mvc;
using QuoteGeneration.Api.Wrappers;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;

namespace QuoteGeneration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RiskAssessmentController : ControllerBase
    {
        private readonly IRiskAssessmentService service;

        public RiskAssessmentController(IRiskAssessmentService service)
        {
            this.service = service;
        }

        [HttpPost("Assess")]
        public async Task<IActionResult> AssessRisk(RiskAssessmentDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<RiskAssessmentDTO>.ErrorResponse(
                    "Invalid risk assessment details."));
            }

            var result = await service.AssessRiskAsync(dto);

            return Ok(ApiResponse<RiskAssessmentDTO>.SuccessResponse(
                result, "Risk assessment completed successfully."));
        }

        [HttpGet("GetByQuoteId/{quoteId}")]
        public async Task<IActionResult> GetRiskByQuoteId(int quoteId)
        {
            var result = await service.GetRiskByQuoteIdAsync(quoteId);

            if (result == null)
            {
                return NotFound(ApiResponse<RiskAssessmentDTO>.ErrorResponse(
                    "Risk assessment not found."));
            }

            return Ok(ApiResponse<RiskAssessmentDTO>.SuccessResponse(
                result, "Risk assessment retrieved successfully."));
        }
    }
}
