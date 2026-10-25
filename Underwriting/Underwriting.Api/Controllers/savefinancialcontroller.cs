
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Underwriting.Api.Wrappers;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;

namespace Underwriting.Api.Controllers
{
    [Route("api/v1/underwriting/{id}/financial")]
    [ApiController]
   // [Authorize]
    public class savefinancialcontroller : ControllerBase
    {
        private readonly Isavefinancial service;

        public savefinancialcontroller(Isavefinancial service)
        {
            this.service = service;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> addfinancial(int id, savefinancialdto dto)
        {
            await service.addfinancial(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse("Financial details saved successfully"));
        }

        [HttpGet]
      //  [Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> fetchfinancial(int id)
        {
            var data = await service.fetchfinancial(id);

            if (data == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Financial details not found"));
            }

            return Ok(ApiResponse<financial_underwriting>.SuccessResponse(data));
        }

        [HttpPut]
       // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> updatefinancial(int id, savefinancialdto dto)
        {
            await service.updatefinancial(id, dto);

            return Ok(ApiResponse<string>.SuccessResponse("Financial details updated successfully"));
        }
    }
}
