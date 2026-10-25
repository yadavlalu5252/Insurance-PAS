
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
    public class QuoteController : ControllerBase
    {
        private readonly IQuoteService service;

        public QuoteController(IQuoteService service)
        {
            this.service = service;
        }

        [HttpPost("Create")]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> CreateQuote(QuoteDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<QuoteDTO>.ErrorResponse("Invalid quote details."));
            }

            var quote = await service.CreateQuoteAsync(dto);

            if (quote == null)
            {
                return BadRequest(ApiResponse<QuoteDTO>.ErrorResponse("Product validation failed. Check the product ID, status, and sum insured."));
            }

            return Ok(ApiResponse<QuoteDTO>.SuccessResponse(quote, "Quote created successfully."));
        }

        [HttpGet("Get/{id}")]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetQuoteById(int id)
        {
            var quote = await service.GetQuoteByIdAsync(id);

            if (quote == null)
            {
                return NotFound(ApiResponse<QuoteDTO>.ErrorResponse("Quote not found."));
            }
            return Ok(ApiResponse<QuoteDTO>.SuccessResponse(quote, "Quote retrieved successfully."));
        }

        [HttpGet("GetAll")]
        //[Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> GetAllQuotes()
        {
            var quotes = await service.GetAllQuotesAsync();


            return Ok(ApiResponse<List<QuoteDTO>>.SuccessResponse(quotes, "Quotes retrieved successfully."));
        }
    }
}
