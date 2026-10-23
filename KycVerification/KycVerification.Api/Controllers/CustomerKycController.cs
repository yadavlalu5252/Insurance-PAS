using KycVerification.Api.Wrappers;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KycVerifiaction.Api.Controllers
{
    [Route("api/v1/customer-kyc")]
    [ApiController]
    [Authorize]
    public class CustomerKycController : ControllerBase
    {
        private readonly ICustomerKycServices service;

        public CustomerKycController(ICustomerKycServices service)
        {
            this.service = service;
        }

        // GET: api/v1/customer-kyc
        // Agent/Admin
        [HttpGet]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAllAsync();

            return Ok(new ApiResponse<List<CustomerKyc>>(
                true,
                "KYC records retrieved successfully.",
                data
            ));
        }

        // GET: api/v1/customer-kyc/5
        // Customer/Agent/Admin
        [HttpGet("{kycId:int}")]
        [Authorize(Roles = "Customer,Agent,Admin")]
        public async Task<IActionResult> GetById(int kycId)
        {
            var data = await service.GetByIdAsync(kycId);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    $"KYC record with ID {kycId} not found.",
                    null,
                    "The requested KYC record does not exist."
                ));
            }

            return Ok(new ApiResponse<CustomerKyc>(
                true,
                "KYC record retrieved successfully.",
                data
            ));
        }

        // GET: api/v1/customer-kyc/customer/1
        // Customer/Agent/Admin
        [HttpGet("customer/{customerId:int}")]
        [Authorize(Roles = "Customer,Agent,Admin")]
        public async Task<IActionResult> GetByCustomerId(int customerId)
        {
            var data = await service.GetByCustomerIdAsync(customerId);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    $"KYC record for customer ID {customerId} not found.",
                    null,
                    "No KYC record exists for the specified customer."
                ));
            }

            return Ok(new ApiResponse<CustomerKyc>(
                true,
                "Customer KYC retrieved successfully.",
                data
            ));
        }

        // POST: api/v1/customer-kyc
        // Agent/Admin
        [HttpPost]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Create(
            [FromBody] CustomerKyc kyc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid KYC data.",
                    null,
                    "One or more validation errors occurred."
                ));
            }

            kyc.kyc_id = 0;
            kyc.kyc_number = $"KYC-{Guid.NewGuid():N}";
            kyc.kyc_date = DateTime.UtcNow;
            kyc.verification_status = "Pending";
            kyc.verified_by = null;
            kyc.verified_date = null;

            var saved = await service.AddAsync(kyc);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<CustomerKyc>(
                    true,
                    "KYC request created successfully.",
                    saved
                )
            );
        }

        // PUT: api/v1/customer-kyc/5
        // Agent/Admin
        [HttpPut("{kycId:int}")]
        [Authorize(Roles = "Agent,Admin")]
        public async Task<IActionResult> Update(
            int kycId,
            [FromBody] CustomerKyc kyc)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid KYC data.",
                    null,
                    "One or more validation errors occurred."
                ));
            }

            var existing = await service.GetByIdAsync(kycId);

            if (existing == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    $"KYC record with ID {kycId} not found.",
                    null,
                    "The requested KYC record does not exist."
                ));
            }

            existing.remarks = kyc.remarks;

            await service.UpdateAsync(existing);

            return Ok(new ApiResponse<CustomerKyc>(
                true,
                "KYC record updated successfully.",
                existing
            ));
        }
    }
}