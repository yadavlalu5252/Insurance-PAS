
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/insurance-products")]
    [ApiController]
    [Authorize]
    public class InsuranceProductController : ControllerBase
    {
        private readonly IInsuranceProductService service;

        public InsuranceProductController(IInsuranceProductService service)
        {
            this.service = service;
        }

       
        [HttpGet]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var products = await service.GetAllAsync();

            return Ok(SuccessResponse.Create("Products retrieved successfully", products ));
        }

        
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await service.GetByIdAsync(id);

            if (product == null)
            {
                return NotFound(ErrorResponse.Create<InsuranceProductDTO>("Product not found"));
            }

            return Ok(SuccessResponse.Create("Product retrieved successfully", product ));
        }

     
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(InsuranceProductDTO dto)
        {
            var product = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create("Product created successfully",product));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, InsuranceProductDTO dto)
        {
            var product = await service.UpdateAsync(id, dto);

            if (product == null)
            {
                return NotFound(ErrorResponse.Create<InsuranceProductDTO>("Product not found"));
            }

            return Ok(SuccessResponse.Create( "Product updated successfully", product));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>( "Product not found"));
            }

            return Ok(SuccessResponse.Create<object>("Product deactivated successfully", null
            ));
        }
    }
}
