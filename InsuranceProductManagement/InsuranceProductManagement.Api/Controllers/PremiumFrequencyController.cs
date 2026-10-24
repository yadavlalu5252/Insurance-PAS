
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/premium-frequencies")]
    [ApiController]
    [Authorize]
    public class PremiumFrequencyController : ControllerBase
    {
        private readonly IPremiumFrequencyService service;

        public PremiumFrequencyController(IPremiumFrequencyService service)
        {
            this.service = service;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var frequencies = await service.GetAllAsync();

            return Ok(SuccessResponse.Create("Premium frequencies retrieved successfully", frequencies));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var frequency = await service.GetByIdAsync(id);

            if (frequency == null)
            {
                return NotFound(ErrorResponse.Create<PremiumFrequencyDTO>("Premium frequency not found"));
            }

            return Ok(SuccessResponse.Create("Premium frequency retrieved successfully", frequency));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(PremiumFrequencyDTO dto)
        {
            var frequency = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create("Premium frequency created successfully",frequency));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, PremiumFrequencyDTO dto)
        {
            var frequency = await service.UpdateAsync(id, dto);

            if (frequency == null)
            {
                return NotFound(ErrorResponse.Create<PremiumFrequencyDTO>( "Premium frequency not found"));
            }

            return Ok(SuccessResponse.Create("Premium frequency updated successfully", frequency));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>("Premium frequency not found"));
            }

            return Ok(SuccessResponse.Create<object>("Premium frequency deactivated successfully",null ));
        }
    }
}
