using KycVerification.Api.Wrappers;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KycVerifiaction.Api.Controllers
{
    [Route("api/v1/kyc/ckyc")]
    [ApiController]
    [Authorize]
    public class CkycDetailsController : ControllerBase
    {
        private readonly ICkycDetailsServices _service;

        public CkycDetailsController(ICkycDetailsServices service)
        {
            _service = service;
        }

        // GET: api/v1/kyc/ckyc/customer/1
        // Customer: own CKYC
        // Agent/Admin: allowed
        [HttpGet("customer/{customerId:int}")]
        [Authorize(Roles = "Customer,Agent,Admin")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var data = await _service.GetByCustomerIdAsync(customerId);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "CKYC details not found.",
                    null,
                    $"No CKYC record exists for customer ID {customerId}."
                ));
            }

            return Ok(new ApiResponse<CkycDetails>(
                true,
                "CKYC details retrieved successfully.",
                data
            ));
        }

        // POST: api/v1/kyc/ckyc
        // Agent/Admin
        [HttpPost]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CkycDetails details)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid CKYC data.",
                    null,
                    "Please check the submitted fields."
                ));
            }

            details.CkycId = 0;

            var saved = await _service.AddAsync(details);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<CkycDetails>(
                    true,
                    "CKYC details created successfully.",
                    saved
                )
            );
        }

        // PUT: api/v1/kyc/ckyc/1
        // Agent/Admin
        [HttpPut("{ckycId:int}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Update(
            int ckycId,
            [FromBody] CkycDetails details)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid CKYC data.",
                    null,
                    "Please check the submitted fields."
                ));
            }

            if (ckycId != details.CkycId)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "CKYC ID mismatch.",
                    null,
                    "The route ID must match the CKYC ID in the request body."
                ));
            }

            var existing = await _service.GetByCustomerIdAsync(
                details.CustomerId);

            if (existing == null || existing.CkycId != ckycId)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "CKYC record not found.",
                    null,
                    $"No CKYC record exists with ID {ckycId}."
                ));
            }

            await _service.UpdateAsync(details);

            return Ok(new ApiResponse<CkycDetails>(
                true,
                "CKYC details updated successfully.",
                details
            ));
        }
    }
}