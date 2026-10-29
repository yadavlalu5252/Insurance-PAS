using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Api.Wrappers;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System.Security.Claims;

namespace PolicyServicing.API.Controllers;

[ApiController]
[Route("api/v1/endorsements")]
[Authorize(Roles = "Admin")]
public class EndorsementController : ControllerBase
{
    private readonly IEndorsementService endorsementService;

    public EndorsementController(IEndorsementService endorsementService)
    {
        this.endorsementService = endorsementService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateEndorsementDTO dto)
    {
        if (dto == null)
            return BadRequest(ApiResponse<object>.ErrorResponse("Invalid data."));

        var result = await endorsementService.CreateEndorsementAsync(dto);

        // TODO: replace `object` with the real return type; confirm `endorsement_number` casing
        return StatusCode(201, ApiResponse<object>.SuccessResponse(
            new { endorsementNumber = result.endorsement_number },
            "Endorsement created successfully."));
    }

    [HttpPut("{id:long}/approve")]
    public async Task<IActionResult> Approve(long id)
    {
        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(adminIdClaim, out long adminId))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Admin ID missing from token."));

        var success = await endorsementService.ApproveEndorsementAsync(id, adminId);

        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse("Endorsement not found."));

        return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Endorsement approved successfully."));
    }

    [HttpPut("{id:long}/reject")]
    public async Task<IActionResult> Reject(long id, [FromBody] RejectEndorsementDTO dto)
    {
        if (dto == null || string.IsNullOrWhiteSpace(dto.RejectionReason))
            return BadRequest(ApiResponse<object>.ErrorResponse("Rejection reason is mandatory."));

        var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!long.TryParse(adminIdClaim, out long adminId))
            return Unauthorized(ApiResponse<object>.ErrorResponse("Admin ID missing from token."));

        var success = await endorsementService.RejectEndorsementAsync(id, adminId, dto.RejectionReason);

        if (!success)
            return NotFound(ApiResponse<object>.ErrorResponse("Endorsement not found."));

        return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Endorsement rejected successfully."));
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> Get(long id)
    {
        var result = await endorsementService.GetEndorsementByIdAsync(id);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Endorsement not found."));

        return Ok(ApiResponse<object>.SuccessResponse(result, "Endorsement retrieved successfully."));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await endorsementService.GetAllPendingEndorsementsAsync();

        return Ok(ApiResponse<object>.SuccessResponse(result, "Endorsements retrieved successfully."));
    }
}