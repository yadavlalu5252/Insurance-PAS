
using Microsoft.AspNetCore.Mvc;
using Underwriting.Api.Wrappers;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;

namespace Underwriting.Api.Controllers
{
    [Route("api/v1/underwriting/{id}/financial")]
    [ApiController]
    public class savefinancialcontroller : ControllerBase
    {
        Isavefinancial service;

        public savefinancialcontroller(Isavefinancial service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> addfinancial(long id, savefinancialdto dto)
        {
            await service.addfinancial(id, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Financial details saved successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> fetchfinancial(long id)
        {
            var data=await service.fetchfinancial(id);
            if (data == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Financial details not found"));
            }

            return Ok(ApiResponse<financial_underwriting>.SuccessResponse(data));
        }
        [HttpPut]
        public async Task<IActionResult> updatefinancial(long id, savefinancialdto dto)
        {
            await service.updatefinancial(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse("Financial details updated successfully"));
        }
    }
}
