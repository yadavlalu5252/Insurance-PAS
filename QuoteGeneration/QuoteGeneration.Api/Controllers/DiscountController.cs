
using Microsoft.AspNetCore.Mvc;
using QuoteGeneration.Api.Wrappers;

using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;

namespace QuoteGeneration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IDiscountService service;

        public DiscountController(IDiscountService service)
        {
            this.service = service;
        }

        [HttpGet("GetActive")]
        public async Task<IActionResult> GetActiveDiscounts()
        {
            var result = await service.GetActiveDiscountsAsync();

            return Ok(ApiResponse<List<DiscountMasterDTO>>.SuccessResponse(
                result, "Active discounts retrieved successfully."));
        }
    }
}
