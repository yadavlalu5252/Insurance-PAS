
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Underwriting.Api.Wrappers;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;

namespace Underwriting.Api.Controllers
{
    [Route("api/v1/underwriting")]
    [ApiController]
    [Authorize]
    public class underwritingcontroller : ControllerBase
    {
        private readonly Iunderwriting service;

        public underwritingcontroller(Iunderwriting service)
        {
            this.service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Agent")]
        public async Task<IActionResult> underwritng(createunderwritingdto dto)
        {
            await service.addunerwriting(dto);
            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case created successfully"));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> fetchunderwriting()
        {
            var data = await service.fetchunderwriting();
            return Ok(ApiResponse<List<underwriting_case>>.SuccessResponse(data));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> fetchunderwritingbyid(int id)
        {
            var data = await service.fetchunderwritingbyid(id);

            if (data == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Underwriting case not found"));
            }

            return Ok(ApiResponse<underwriting_case>.SuccessResponse(data));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updateunderwritingbyid(int id, createunderwritingdto dto)
        {
            await service.updateunderwritingbyid(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case updated successfully"));
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> approveunderwriting(int id)
        {
            await service.approveunderwriting(id);

            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case approved successfully"));
        }

        [HttpPut("{id}/reject")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> rejectunderwriting(
            int id, [FromBody] string reason)
        {
            await service.rejectunderwriting(id, reason);

            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case rejected successfully"));
        }

        [HttpPut("{id}/loading")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> loadingunderwriting(int id)
        {
            await service.loadingunderwriting(id);

            return Ok(ApiResponse<string>.SuccessResponse("Premium loading applied successfully"));
        }

        [HttpGet("{id}/decision")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> fetchdecision(int id)
        {
            var data = await service.fetchdecision(id);

            if (data != null)
            {
                return Ok(ApiResponse<underwriting_decision>.SuccessResponse( data, "Underwriting decision fetched successfully"));
            }

            return NotFound(ApiResponse<string>.ErrorResponse("Underwriting decision not found"));
        }

        [HttpGet("quote/{quoteId}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> fetchunderwritingbyquoteid(int quoteId)
        {
            var data = await service.fetchunderwritingbyquoteid(quoteId);

            if (data == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Underwriting case not found for this quote"));
            }

            return Ok(ApiResponse<underwriting_case>.SuccessResponse(data));
        }
    }
}
