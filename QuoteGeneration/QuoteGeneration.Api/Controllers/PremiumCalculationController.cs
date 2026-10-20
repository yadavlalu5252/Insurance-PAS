
using Microsoft.AspNetCore.Mvc;
using QuoteGeneration.Api.Wrappers;

using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;

namespace QuoteGeneration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PremiumCalculationController : ControllerBase
    {
        private readonly IPremiumCalculationService service;

        public PremiumCalculationController(IPremiumCalculationService service)
        {
            this.service = service;
        }

        [HttpPost("Calculate/{quoteId}")]
        public async Task<IActionResult> CalculatePremium(int quoteId)
        {
            var result = await service.CalculatePremiumAsync(quoteId);

            return Ok(ApiResponse<PremiumCalculationDTO>.SuccessResponse(
                result, "Premium calculated successfully."));
        }

        [HttpGet("GetByQuoteId/{quoteId}")]
        public async Task<IActionResult> GetPremiumByQuoteId(int quoteId)
        {
            var result = await service.GetPremiumByQuoteIdAsync(quoteId);

            if (result == null)
            {
                return NotFound(ApiResponse<PremiumCalculationDTO>.ErrorResponse(
                    "Premium calculation not found."));
            }

            return Ok(ApiResponse<PremiumCalculationDTO>.SuccessResponse(
                result, "Premium calculation retrieved successfully."));
        }
    }
}
