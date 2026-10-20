using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/premium-frequencies")]
    [ApiController]
    public class PremiumFrequencyController : ControllerBase
    {
        private readonly IPremiumFrequencyService service;

        public PremiumFrequencyController(IPremiumFrequencyService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var frequencies = await service.GetAllAsync();

            return Ok(ApiResponse<List<PremiumFrequencyDTO>>.SuccessResponse(
                frequencies, "Premium frequencies retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var frequency = await service.GetByIdAsync(id);

            if (frequency == null)
                return NotFound(ApiResponse<PremiumFrequencyDTO>.ErrorResponse("Premium frequency not found"));

            return Ok(ApiResponse<PremiumFrequencyDTO>.SuccessResponse(
                frequency, "Premium frequency retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(PremiumFrequencyDTO dto)
        {
            var frequency = await service.CreateAsync(dto);

            return Ok(ApiResponse<PremiumFrequencyDTO>.SuccessResponse(
                frequency, "Premium frequency created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, PremiumFrequencyDTO dto)
        {
            var frequency = await service.UpdateAsync(id, dto);

            if (frequency == null)
                return NotFound(ApiResponse<PremiumFrequencyDTO>.ErrorResponse("Premium frequency not found"));

            return Ok(ApiResponse<PremiumFrequencyDTO>.SuccessResponse(
                frequency, "Premium frequency updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Premium frequency not found"));

            return Ok(ApiResponse<object>.SuccessResponse(
                null, "Premium frequency deactivated successfully"));
        }
    }
}
