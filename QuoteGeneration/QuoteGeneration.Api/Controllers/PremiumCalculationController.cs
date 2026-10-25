
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using QuoteGeneration.Api.Wrappers;
using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;

namespace QuoteGeneration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class PremiumCalculationController : ControllerBase
    {
        private readonly IPremiumCalculationService service;

        public PremiumCalculationController(
            IPremiumCalculationService service)
        {
            this.service = service;
        }

        [HttpPost("Calculate/{quoteId}")]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> CalculatePremium(int quoteId)
        {
            var result = await service.CalculatePremiumAsync(quoteId);

            if (result == null)
            {
                return BadRequest(ApiResponse<PremiumCalculationDTO>.ErrorResponse("Premium calculation failed. Check quote, product, and underwriting details."));
            }

            return Ok(ApiResponse<PremiumCalculationDTO>.SuccessResponse( result, "Premium calculated successfully."));
        }

        [HttpGet("GetByQuoteId/{quoteId}")]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetPremiumByQuoteId(int quoteId)
        {
            var result = await service.GetPremiumByQuoteIdAsync(quoteId);

            if (result == null)
            {
                return NotFound(ApiResponse<PremiumCalculationDTO>.ErrorResponse("Premium calculation not found."));
            }

            return Ok( ApiResponse<PremiumCalculationDTO>.SuccessResponse(result, "Premium calculation retrieved successfully."));
        }
    }
}
