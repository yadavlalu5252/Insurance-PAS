
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/benefits")]
    [ApiController]
    [Authorize]
    public class BenefitMasterController : ControllerBase
    {
        private readonly IBenefitMasterService service;

        public BenefitMasterController(IBenefitMasterService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var benefits = await service.GetAllAsync();

            return Ok(SuccessResponse.Create("Benefits retrieved successfully",benefits));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var benefit = await service.GetByIdAsync(id);

            if (benefit == null)
            {
                return NotFound(ErrorResponse.Create<BenefitMasterDTO>("Benefit not found"));
            }

            return Ok(SuccessResponse.Create("Benefit retrieved successfully",benefit));
        }

   
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(BenefitMasterDTO dto)
        {
            var benefit = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create("Benefit created successfully",benefit));
        }


        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, BenefitMasterDTO dto)
        {
            var benefit = await service.UpdateAsync(id, dto);

            if (benefit == null)
            {
                return NotFound(ErrorResponse.Create<BenefitMasterDTO>("Benefit not found"));
            }

            return Ok(SuccessResponse.Create( "Benefit updated successfully",benefit));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>("Benefit not found"));
            }

            return Ok(SuccessResponse.Create<object>("Benefit deleted successfully",null));
        }
    }
}
