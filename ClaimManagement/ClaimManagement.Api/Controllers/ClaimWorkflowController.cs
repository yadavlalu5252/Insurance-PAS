using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/claims/{id}")]
    [ApiController]
    public class ClaimWorkflowController : ControllerBase
    {
        IClaimWorkflowService service;
        ILogger<ClaimWorkflowController> logger;

        // Temporary user id until JWT login is added
        const long TempUserId = 0;

        public ClaimWorkflowController(IClaimWorkflowService service, ILogger<ClaimWorkflowController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        // Admin only once JWT is added
        [HttpPut]
        [Route("approve")]
        public async Task<IActionResult> ApproveClaim(long id, ApproveClaimDTO dto)
        {
            var result = await service.ApproveClaimAsync(id, dto, TempUserId);

            if (result == "NotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "InvalidStatus")
                return Conflict(ApiResponse<object>.Fail("Claim cannot be approved in its current status"));

            if (result == "DocumentsNotVerified")
                return Conflict(ApiResponse<object>.Fail("All claim documents must be uploaded and verified before approval"));

            if (result == "SurveyPending")
                return Conflict(ApiResponse<object>.Fail("The survey report must be submitted before approval"));

            logger.LogInformation("Claim {ClaimId} approved", id);

            return Ok(ApiResponse<object>.Ok(null, "Claim approved successfully"));
        }

        // Admin only once JWT is added
        [HttpPut]
        [Route("reject")]
        public async Task<IActionResult> RejectClaim(long id, RejectClaimDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.reason))
                return BadRequest(ApiResponse<object>.Fail("Reason is required to reject a claim"));

            var result = await service.RejectClaimAsync(id, dto, TempUserId);

            if (result == "NotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "InvalidStatus")
                return Conflict(ApiResponse<object>.Fail("Claim cannot be rejected in its current status"));

            logger.LogInformation("Claim {ClaimId} rejected", id);

            return Ok(ApiResponse<object>.Ok(null, "Claim rejected successfully"));
        }

        [HttpDelete]
        public async Task<IActionResult> CancelClaim(long id)
        {
            var result = await service.CancelClaimAsync(id, TempUserId);

            if (result == "NotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "InvalidStatus")
                return Conflict(ApiResponse<object>.Fail("Claim can be cancelled only before processing starts"));

            logger.LogInformation("Claim {ClaimId} cancelled", id);

            return Ok(ApiResponse<object>.Ok(null, "Claim cancelled successfully"));
        }
    }
}
