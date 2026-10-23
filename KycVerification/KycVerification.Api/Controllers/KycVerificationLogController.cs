using KycVerification.Api.Wrappers;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KycVerifiaction.Api.Controllers
{
    [Route("api/v1/kyc/logs")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class KycVerificationLogController : ControllerBase
    {
        private readonly IKycVerificationLogServices service;

        public KycVerificationLogController(
            IKycVerificationLogServices service)
        {
            this.service = service;
        }

        // GET: api/v1/kyc/logs
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAllAsync();

            return Ok(new ApiResponse<List<KycVerificationLog>>(
                true,
                "KYC audit logs retrieved successfully.",
                data
            ));
        }

        // GET: api/v1/kyc/logs/5
        [HttpGet("{logId:int}")]
        public async Task<IActionResult> GetById(int logId)
        {
            var data = await service.GetByIdAsync(logId);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Audit log not found.",
                    null,
                    $"No audit log exists with ID {logId}."
                ));
            }

            return Ok(new ApiResponse<KycVerificationLog>(
                true,
                "Audit log retrieved successfully.",
                data
            ));
        }

        // GET: api/v1/kyc/logs/kyc/5
        [HttpGet("kyc/{kycId:int}")]
        public async Task<IActionResult> GetByKycId(int kycId)
        {
            var data = await service.GetByKycIdAsync(kycId);

            return Ok(new ApiResponse<List<KycVerificationLog>>(
                true,
                "KYC audit history retrieved successfully.",
                data
            ));
        }
    }
}