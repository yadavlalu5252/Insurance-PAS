using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/riders")]
    [ApiController]
    public class RiderMasterController : ControllerBase
    {
        private readonly IRiderMasterService service;

        public RiderMasterController(IRiderMasterService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var riders = await service.GetAllAsync();

            return Ok(ApiResponse<List<RiderMasterDTO>>.SuccessResponse(
                riders, "Riders retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rider = await service.GetByIdAsync(id);

            if (rider == null)
                return NotFound(ApiResponse<RiderMasterDTO>.ErrorResponse("Rider not found"));

            return Ok(ApiResponse<RiderMasterDTO>.SuccessResponse(
                rider, "Rider retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(RiderMasterDTO dto)
        {
            var rider = await service.CreateAsync(dto);

            return Ok(ApiResponse<RiderMasterDTO>.SuccessResponse(
                rider, "Rider created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RiderMasterDTO dto)
        {
            var rider = await service.UpdateAsync(id, dto);

            if (rider == null)
                return NotFound(ApiResponse<RiderMasterDTO>.ErrorResponse("Rider not found"));

            return Ok(ApiResponse<RiderMasterDTO>.SuccessResponse(
                rider, "Rider updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Rider not found"));

            return Ok(ApiResponse<object>.SuccessResponse(
                null, "Rider deactivated successfully"));
        }
    }
}
