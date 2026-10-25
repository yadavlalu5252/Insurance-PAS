
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using InsuranceProductManagement.Application.DTO;
using InsuranceProductManagement.Application.Interface;
using InsuranceProductManagement.Application.Common;

namespace InsuranceProductManagement.Api.Controllers
{
    [Route("api/v1/coverages")]
    [ApiController]
    //[Authorize]
    public class CoverageMasterController : ControllerBase
    {
        private readonly ICoverageMasterService service;

        public CoverageMasterController(ICoverageMasterService service)
        {
            this.service = service;
        }

        [HttpGet]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetAll()
        {
            var coverages = await service.GetAllAsync();

            return Ok(SuccessResponse.Create("Coverages retrieved successfully",coverages));
        }

        [HttpGet("{id}")]
        //[Authorize(Roles = "Admin,Agent,Customer")]
        public async Task<IActionResult> GetById(int id)
        {
            var coverage = await service.GetByIdAsync(id);

            if (coverage == null)
            {
                return NotFound(ErrorResponse.Create<CoverageMasterDTO>("Coverage not found"));
            }

            return Ok(SuccessResponse.Create("Coverage retrieved successfully",coverage));
        }

      
        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(CoverageMasterDTO dto)
        {
            var coverage = await service.CreateAsync(dto);

            return Ok(SuccessResponse.Create("Coverage created successfully",coverage));
        }

        [HttpPut("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, CoverageMasterDTO dto)
        {
            var coverage = await service.UpdateAsync(id, dto);

            if (coverage == null)
            {
                return NotFound(ErrorResponse.Create<CoverageMasterDTO>("Coverage not found" ));
            }

            return Ok(SuccessResponse.Create("Coverage updated successfully", coverage ));
        }

     
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await service.DeleteAsync(id);

            if (!result)
            {
                return NotFound(ErrorResponse.Create<object>("Coverage not found" ));
            }

            return Ok(SuccessResponse.Create<object>( "Coverage deleted successfully",null));
        }
    }
}
