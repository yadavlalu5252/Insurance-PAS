using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/product-commissions")]
    [ApiController]
    public class ProductCommissionController : ControllerBase
    {
        private readonly IProductCommissionService service;

        public ProductCommissionController(IProductCommissionService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var commissions = await service.GetAllAsync();

            return Ok(ApiResponse<List<ProductCommissionDTO>>.SuccessResponse(
                commissions, "Product commissions retrieved successfully"));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var commission = await service.GetByIdAsync(id);

            if (commission == null)
                return NotFound(ApiResponse<ProductCommissionDTO>.ErrorResponse("Product commission not found"));

            return Ok(ApiResponse<ProductCommissionDTO>.SuccessResponse(
                commission, "Product commission retrieved successfully"));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCommissionDTO dto)
        {
            var commission = await service.CreateAsync(dto);

            return Ok(ApiResponse<ProductCommissionDTO>.SuccessResponse(
                commission, "Product commission created successfully"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCommissionDTO dto)
        {
            var commission = await service.UpdateAsync(id, dto);

            if (commission == null)
                return NotFound(ApiResponse<ProductCommissionDTO>.ErrorResponse("Product commission not found"));

            return Ok(ApiResponse<ProductCommissionDTO>.SuccessResponse(
                commission, "Product commission updated successfully"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
                return NotFound(ApiResponse<object>.ErrorResponse("Product commission not found"));

            return Ok(ApiResponse<object>.SuccessResponse(
                null, "Product commission deleted successfully"));
        }
    }
}
