using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/coverages")]
    [ApiController]
    public class CoverageMasterController : ControllerBase
    {
        private readonly ICoverageMasterService service;

        public CoverageMasterController(ICoverageMasterService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var coverages = await service.GetAllAsync();

            return Ok(ApiResponse<List<CoverageMasterDTO>>.SuccessResponse(
                coverages, "Coverages retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var coverage = await service.GetByIdAsync(id);

            if (coverage == null)
                return NotFound(ApiResponse<CoverageMasterDTO>.ErrorResponse("Coverage not found"));

            return Ok(ApiResponse<CoverageMasterDTO>.SuccessResponse(
                coverage, "Coverage retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CoverageMasterDTO dto)
        {
            var coverage = await service.CreateAsync(dto);

            return Ok(ApiResponse<CoverageMasterDTO>.SuccessResponse(
                coverage, "Coverage created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CoverageMasterDTO dto)
        {
            var coverage = await service.UpdateAsync(id, dto);

            if (coverage == null)
                return NotFound(ApiResponse<CoverageMasterDTO>.ErrorResponse("Coverage not found"));

            return Ok(ApiResponse<CoverageMasterDTO>.SuccessResponse(
                coverage, "Coverage updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Coverage not found"));

            return Ok(ApiResponse<object>.SuccessResponse(
                null, "Coverage deleted successfully"));
        }
    }
}
