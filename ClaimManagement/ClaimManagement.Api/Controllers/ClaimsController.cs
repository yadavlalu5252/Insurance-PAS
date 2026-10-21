using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/claims")]
    [ApiController]
    public class ClaimsController : ControllerBase
    {
        IClaimService service;
        ILogger<ClaimsController> logger;

        // Temporary user id until JWT login is added
        const long TempUserId = 0;

        public ClaimsController(IClaimService service, ILogger<ClaimsController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddClaim(ClaimCreateDto dto)
        {
            if (dto.policy_id <= 0)
                return BadRequest(ApiResponse<object>.Fail("Policy id is required"));

            if (dto.customer_id <= 0)
                return BadRequest(ApiResponse<object>.Fail("Customer id is required"));

            if (string.IsNullOrWhiteSpace(dto.claim_type))
                return BadRequest(ApiResponse<object>.Fail("Claim type is required"));

            if (dto.incident_date.Date > DateTime.Now.Date)
                return BadRequest(ApiResponse<object>.Fail("Incident date cannot be in the future"));

            if (string.IsNullOrWhiteSpace(dto.description))
                return BadRequest(ApiResponse<object>.Fail("Description is required"));

            if (dto.estimated_loss <= 0)
                return BadRequest(ApiResponse<object>.Fail("Estimated loss must be greater than 0"));

            var (result, claim, reason) = await service.AddClaimAsync(dto, TempUserId);

            if (result == "NotEligible")
                return UnprocessableEntity(ApiResponse<object>.Fail(reason ?? "Claim is not eligible"));

            if (result == "ServiceUnavailable")
                return StatusCode(503, ApiResponse<object>.Fail(reason ?? "Service not available"));

            logger.LogInformation("Claim {ClaimNumber} registered", claim!.claim_number);

            return StatusCode(201, ApiResponse<ClaimResponseDto>.Ok(claim, "Claim registered successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> ClaimList()
        {
            var d = await service.FetchClaimsAsync();
            return Ok(ApiResponse<List<ClaimResponseDto>>.Ok(d));
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ClaimById(long id)
        {
            var d = await service.FetchClaimByIdAsync(id);

            if (d == null)
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            return Ok(ApiResponse<ClaimResponseDto>.Ok(d));
        }

        [HttpGet]
        [Route("{id}/status")]
        public async Task<IActionResult> ClaimStatus(long id)
        {
            var d = await service.FetchClaimStatusAsync(id);

            if (d == null)
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            return Ok(ApiResponse<ClaimStatusResponseDto>.Ok(d));
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateClaim(long id, ClaimUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.description))
                return BadRequest(ApiResponse<object>.Fail("Description is required"));

            if (dto.estimated_loss <= 0)
                return BadRequest(ApiResponse<object>.Fail("Estimated loss must be greater than 0"));

            var result = await service.UpdateClaimAsync(id, dto, TempUserId);

            if (result == "NotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "NotAllowed")
                return Conflict(ApiResponse<object>.Fail("Claim cannot be updated in its current status"));

            logger.LogInformation("Claim {ClaimId} updated", id);

            return Ok(ApiResponse<object>.Ok(null, "Claim updated successfully"));
        }

        [HttpGet]
        [Route("{id}/history")]
        public async Task<IActionResult> ClaimHistory(long id)
        {
            var d = await service.FetchClaimHistoryAsync(id);

            if (d == null)
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            return Ok(ApiResponse<List<ClaimHistoryDTO>>.Ok(d));
        }
    }
}
