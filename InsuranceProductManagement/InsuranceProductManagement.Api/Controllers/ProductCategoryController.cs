
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/product-categories")]
    [ApiController]
    [Authorize]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IProductCategoryService service;

        public ProductCategoryController(IProductCategoryService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var categories = await service.GetAllAsync();

            return Ok(SuccessResponse.Create("Categories retrieved successfully", categories));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var category = await service.GetByIdAsync(id);

            if (category == null)
            {
                return NotFound(ErrorResponse.Create<ProductCategoryDTO>("Category not found"));
            }

            return Ok(SuccessResponse.Create( "Category retrieved successfully", category));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(ProductCategoryDTO dto)
        {
            var category = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create( "Category created successfully", category));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, ProductCategoryDTO dto)
        {
            var category = await service.UpdateAsync(id, dto);

            if (category == null)
            {
                return NotFound(ErrorResponse.Create<ProductCategoryDTO>("Category not found"));
            }

            return Ok(SuccessResponse.Create( "Category updated successfully", category));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>( "Category not found" ));
            }

            return Ok(SuccessResponse.Create<object>( "Category deactivated successfully", null));
        }
    }
}
