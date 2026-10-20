using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/benefits")]
    [ApiController]
    public class BenefitMasterController : ControllerBase
    {
        private readonly IBenefitMasterService service;

        public BenefitMasterController(IBenefitMasterService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var benefits = await service.GetAllAsync();

            return Ok(ApiResponse<List<BenefitMasterDTO>>.SuccessResponse(
                benefits, "Benefits retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var benefit = await service.GetByIdAsync(id);

            if (benefit == null)
                return NotFound(ApiResponse<BenefitMasterDTO>.ErrorResponse("Benefit not found"));

            return Ok(ApiResponse<BenefitMasterDTO>.SuccessResponse(
                benefit, "Benefit retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(BenefitMasterDTO dto)
        {
            var benefit = await service.CreateAsync(dto);

            return Ok(ApiResponse<BenefitMasterDTO>.SuccessResponse(
                benefit, "Benefit created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, BenefitMasterDTO dto)
        {
            var benefit = await service.UpdateAsync(id, dto);

            if (benefit == null)
                return NotFound(ApiResponse<BenefitMasterDTO>.ErrorResponse("Benefit not found"));

            return Ok(ApiResponse<BenefitMasterDTO>.SuccessResponse(
                benefit, "Benefit updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Benefit not found"));

            return Ok(ApiResponse<object>.SuccessResponse(
                null, "Benefit deleted successfully"));
        }
    }
}
