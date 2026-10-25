
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/product-commissions")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
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

            return Ok(SuccessResponse.Create("Product commissions retrieved successfully",commissions));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var commission = await service.GetByIdAsync(id);

            if (commission == null)
            {
                return NotFound(ErrorResponse.Create<ProductCommissionDTO>("Product commission not found"));
            }

            return Ok(SuccessResponse.Create("Product commission retrieved successfully", commission));
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCommissionDTO dto)
        {
            var commission = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create("Product commission created successfully",commission));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductCommissionDTO dto)
        {
            var commission = await service.UpdateAsync(id, dto);

            if (commission == null)
            {
                return NotFound(ErrorResponse.Create<ProductCommissionDTO>( "Product commission not found" ));
            }

            return Ok(SuccessResponse.Create( "Product commission updated successfully",commission ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>( "Product commission not found"));
            }

            return Ok(SuccessResponse.Create<object>( "Product commission deleted successfully", null));
        }
    }
}
