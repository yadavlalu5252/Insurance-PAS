
using Microsoft.AspNetCore.Mvc;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/riders")]
    [ApiController]
    public class RiderMasterController : ControllerBase
    {
        private readonly IRiderMasterService service;

        public RiderMasterController(IRiderMasterService service)
        {
            this.service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var riders = await service.GetAllAsync();

            return Ok(SuccessResponse.Create(
                "Riders retrieved successfully",
                riders
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rider = await service.GetByIdAsync(id);

            if (rider == null)
            {
                return NotFound(ErrorResponse.Create<RiderMasterDTO>(
                    "Rider not found"
                ));
            }

            return Ok(SuccessResponse.Create(
                "Rider retrieved successfully",
                rider
            ));
        }

        [HttpPost]
        public async Task<IActionResult> Create(RiderMasterDTO dto)
        {
            var rider = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create(
                "Rider created successfully",
                rider
            ));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, RiderMasterDTO dto)
        {
            var rider = await service.UpdateAsync(id, dto);

            if (rider == null)
            {
                return NotFound(ErrorResponse.Create<RiderMasterDTO>(
                    "Rider not found"
                ));
            }

            return Ok(SuccessResponse.Create(
                "Rider updated successfully",
                rider
            ));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>(
                    "Rider not found"
                ));
            }

            return Ok(SuccessResponse.Create<object>(
                "Rider deactivated successfully",
                null
            ));
        }
    }
}
