using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/claims/{id}")]
[ApiController]
public class ClaimWorkflowController : ControllerBase
{
    IClaimWorkflowService service;
    ILogger<ClaimWorkflowController> logger;

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
        await service.ApproveClaimAsync(id, dto, TempUserId);
        logger.LogInformation("Claim {ClaimId} approved", id);
        return Ok(ApiResponse<object>.Ok(null, "Claim approved successfully"));
    }

    // Admin only once JWT is added
    [HttpPut]
    [Route("reject")]
    public async Task<IActionResult> RejectClaim(long id, RejectClaimDTO dto)
    {
        await service.RejectClaimAsync(id, dto, TempUserId);
        logger.LogInformation("Claim {ClaimId} rejected", id);
        return Ok(ApiResponse<object>.Ok(null, "Claim rejected successfully"));
    }

    [HttpDelete]
    public async Task<IActionResult> CancelClaim(long id)
    {
        await service.CancelClaimAsync(id, TempUserId);
        logger.LogInformation("Claim {ClaimId} cancelled", id);
        return Ok(ApiResponse<object>.Ok(null, "Claim cancelled successfully"));
    }
}