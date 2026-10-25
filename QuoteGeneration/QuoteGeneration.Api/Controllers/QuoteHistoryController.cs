
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
    public class QuoteHistoryController : ControllerBase
    {
        private readonly IQuoteHistoryService service;

        public QuoteHistoryController(IQuoteHistoryService service)
        {
            this.service = service;
        }

        [HttpGet("GetByQuoteId/{quoteId}")]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetHistoryByQuoteId(int quoteId)
        {
            var result = await service.GetHistoryByQuoteIdAsync(quoteId);

            return Ok(ApiResponse<List<QuoteHistoryDTO>>.SuccessResponse(result, "Quote history retrieved successfully."));
        }
    }
}
