using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

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
    public async Task<IActionResult> AddClaim(CreateClaimDTO dto)
    {
        var d = await service.AddClaimAsync(dto, TempUserId);
        logger.LogInformation("Claim {ClaimNumber} registered", d.claim_number);
        return StatusCode(201, ApiResponse<ClaimResponseDTO>.Ok(d, "Claim registered successfully"));
    }

    // Example: GET /api/v1/claims?page_number=1&page_size=10&claim_status=Approved&search=CLM
    [HttpGet]
    public async Task<IActionResult> ClaimList([FromQuery] ClaimFilterDTO filter)
    {
        var d = await service.FetchClaimsAsync(filter);
        return Ok(ApiResponse<PagedResultDTO<ClaimResponseDTO>>.Ok(d));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> ClaimById(long id)
    {
        var d = await service.FetchClaimByIdAsync(id);
        return Ok(ApiResponse<ClaimResponseDTO>.Ok(d));
    }

    [HttpGet]
    [Route("{id}/status")]
    public async Task<IActionResult> ClaimStatus(long id)
    {
        var d = await service.FetchClaimStatusAsync(id);
        return Ok(ApiResponse<ClaimStatusDTO>.Ok(d));
    }

    [HttpGet]
    [Route("{id}/history")]
    public async Task<IActionResult> ClaimHistory(long id)
    {
        var d = await service.FetchClaimHistoryAsync(id);
        return Ok(ApiResponse<List<ClaimHistoryDTO>>.Ok(d));
    }

    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> UpdateClaim(long id, UpdateClaimDTO dto)
    {
        await service.UpdateClaimAsync(id, dto, TempUserId);
        logger.LogInformation("Claim {ClaimId} updated", id);
        return Ok(ApiResponse<object>.Ok(null, "Claim updated successfully"));
    }
}