using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/insurance-products")]
    [ApiController]
    public class InsuranceProductController : ControllerBase
    {
        private readonly IInsuranceProductService service;

        public InsuranceProductController(IInsuranceProductService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await service.GetAllAsync();

            return Ok(ApiResponse<List<InsuranceProductDTO>>.SuccessResponse(
                products, "Insurance products retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await service.GetByIdAsync(id);

            if (product == null)
                return NotFound(ApiResponse<InsuranceProductDTO>.ErrorResponse("Insurance product not found"));

            return Ok(ApiResponse<InsuranceProductDTO>.SuccessResponse(
                product, "Insurance product retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(InsuranceProductDTO dto)
        {
            var product = await service.CreateAsync(dto);

            return Ok(ApiResponse<InsuranceProductDTO>.SuccessResponse(
                product, "Insurance product created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, InsuranceProductDTO dto)
        {
            var product = await service.UpdateAsync(id, dto);

            if (product == null)
                return NotFound(ApiResponse<InsuranceProductDTO>.ErrorResponse("Insurance product not found"));

            return Ok(ApiResponse<InsuranceProductDTO>.SuccessResponse(
                product, "Insurance product updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Insurance product not found"));

            return Ok(ApiResponse<object>.SuccessResponse(
                null, "Insurance product deactivated successfully"));
        }
    }
}
