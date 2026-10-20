using InsuranceProductManagement.Api.Wrappers;

using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;

using Microsoft.AspNetCore.Mvc;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/product-categories")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService service;

        public ProductCategoryController(IProductCategoryService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await service.GetAllAsync();

            return Ok(ApiResponse<List<ProductCategoryDTO>>.SuccessResponse(
                categories,
                "Categories retrieved successfully"
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await service.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(ApiResponse<ProductCategoryDTO>.ErrorResponse(
                    "Category not found"
                ));
            }

            return Ok(ApiResponse<ProductCategoryDTO>.SuccessResponse(
                category,
                "Category retrieved successfully"
            ));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCategoryDTO dto)
        {
            var category = await service.CreateAsync(dto);

            return Ok(ApiResponse<ProductCategoryDTO>.SuccessResponse(
                category,
                "Category created successfully"
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCategoryDTO dto)
        {
            var category = await service.UpdateAsync(id, dto);

            if (category == null)
            {
                return NotFound(ApiResponse<ProductCategoryDTO>.ErrorResponse(
                    "Category not found"
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
     null,
     "Category deactivated successfully"
 ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ApiResponse<object>.ErrorResponse(
                    "Category not found"
                ));
            }

            return Ok(ApiResponse<object>.SuccessResponse(
                null,
                "Category deactivated successfully"
            ));
        }
    }
}