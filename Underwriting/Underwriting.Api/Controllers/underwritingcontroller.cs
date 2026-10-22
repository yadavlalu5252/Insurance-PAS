using Microsoft.AspNetCore.Mvc;
using Underwriting.Api.Wrappers;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Underwriting.Api.Controllers
{
    [Route("api/v1/underwriting")]
    [ApiController]
    public class underwritingcontroller : ControllerBase
    {
        Iunderwriting service;

        public underwritingcontroller(Iunderwriting service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> underwritng(createunderwritingdto dto)
        {
            await service.addunerwriting(dto);

            return Ok(ApiResponse<string>.SuccessResponse(
                "Underwriting case created successfully"
            ));
        }

        [HttpGet]
        public async Task<IActionResult> fetchunderwriting()
        {
            var data = await service.fetchunderwriting();

            return Ok(ApiResponse<List<underwriting_case>>.SuccessResponse(data));
        }

        [HttpGet("{id}")]
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
        public async Task<IActionResult> updateunderwritingbyid(int id, createunderwritingdto dto)
        {
            await service.updateunderwritingbyid(id, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case updated successfully"));
        }

        [HttpPut("{id}/approve")]
        public async Task<IActionResult> approveunderwriting(int id)
        {
            await service.approveunderwriting(id);
            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case approved successfully"));
        }
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> rejectunderwriting(int id, [FromBody] string reason)
        {
            await service.rejectunderwriting(id, reason);

            return Ok(ApiResponse<string>.SuccessResponse("Underwriting case rejected successfully"));
        }
        [HttpPut("{id}/loading")]
        public async Task<IActionResult> loadingunderwriting(int id)
        {
            await service.loadingunderwriting(id);

            return Ok(
                ApiResponse<string>.SuccessResponse(
                    "Premium loading applied successfully"
                )
            );
        }
        [HttpGet("{id}/decision")]
        public async Task<IActionResult> fetchdecision(int id)
        {
            var data = await service.fetchdecision(id);

            if (data != null)
            {
                return Ok(ApiResponse<underwriting_decision>.SuccessResponse(data,"Underwriting decision fetched successfully"));
            }
            return NotFound(ApiResponse<string>.ErrorResponse("Underwriting decision not found"));
        }
        [HttpGet("quote/{quoteId}")]
        public async Task<IActionResult> fetchunderwritingbyquoteid(int quoteId)
        {
            var data = await service.fetchunderwritingbyquoteid(quoteId);

            if (data == null)
                return NotFound(ApiResponse<string>.ErrorResponse(
                    "Underwriting case not found for this quote"));

            return Ok(ApiResponse<underwriting_case>.SuccessResponse(data));
        }
    }
}