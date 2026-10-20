
using Microsoft.AspNetCore.Mvc;
using QuoteGeneration.Api.Wrappers;

using QuoteGeneration.Application.DTO;
using QuoteGeneration.Application.Interfaces;

namespace QuoteGeneration.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuoteRiderController : ControllerBase
    {
        private readonly IQuoteRiderService service;

        public QuoteRiderController(IQuoteRiderService service)
        {
            this.service = service;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddRider(QuoteRiderDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<QuoteRiderDTO>.ErrorResponse(
                    "Invalid rider details."));
            }

            var result = await service.AddRiderAsync(dto);

            return Ok(ApiResponse<QuoteRiderDTO>.SuccessResponse(
                result, "Rider added successfully."));
        }

        [HttpGet("GetByQuoteId/{quoteId}")]
        public async Task<IActionResult> GetRidersByQuoteId(int quoteId)
        {
            var result = await service.GetRidersByQuoteIdAsync(quoteId);

            return Ok(ApiResponse<List<QuoteRiderDTO>>.SuccessResponse(
                result, "Quote riders retrieved successfully."));
        }

        [HttpDelete("Remove/{id}")]
        public async Task<IActionResult> RemoveRider(int id)
        {
            var removed = await service.RemoveRiderAsync(id);

            if (!removed)
            {
                return NotFound(ApiResponse<bool>.ErrorResponse(
                    "Rider selection not found."));
            }

            return Ok(ApiResponse<bool>.SuccessResponse(
                true, "Rider removed successfully."));
        }
    }
}
