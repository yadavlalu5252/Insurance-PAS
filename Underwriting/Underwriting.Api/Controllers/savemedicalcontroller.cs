using Microsoft.AspNetCore.Mvc;
using Underwriting.Api.Wrappers;
using Underwriting.Application.DTO;
using Underwriting.Application.Interfaces;
using Underwriting.Domain.Entities;

namespace Underwriting.Api.Controllers
{
    [Route("api/v1/underwriting/{id}/medical")]
    [ApiController]
    public class savemedicalcontroller : ControllerBase
    {
        Isavemedical service;

        public savemedicalcontroller(Isavemedical service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> addmedical(int id, savemedicaldto dto)
        {
            await service.addmedical(id, dto);
            return Ok(ApiResponse<string>.SuccessResponse("Medical details saved successfully"));

        }
        [HttpGet]
        public async Task<IActionResult> fetchmedical(int id)
        {
            var data = await service.fetchmedical(id);
            if (data == null)
            {
                return NotFound(ApiResponse<string>.ErrorResponse("Medical details not found"));
            }
            return Ok(ApiResponse<medical_underwriting>.SuccessResponse(data));
        }

        [HttpPut]
        public async Task<IActionResult> updatemedical(int id,savemedicaldto dto)
        {
            await service.updatemedical(id,dto);
            return Ok(ApiResponse<string>.SuccessResponse("Medical details updated successfully"));
        }
    }
}
