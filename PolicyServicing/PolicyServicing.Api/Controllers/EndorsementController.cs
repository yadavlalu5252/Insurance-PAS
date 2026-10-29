using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Api.Wrappers;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PolicyServicing.API.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/v1/endorsements")]
    public class EndorsementController : ControllerBase
    {
        private readonly IEndorsementService _endorsementService;

        public EndorsementController(IEndorsementService endorsementService)
        {
            _endorsementService = endorsementService;
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEndorsementDTO dto)
        {
            if (dto == null) return BadRequest(ApiResponse<object>.ErrorResponse("Invalid data."));

           
                var result = await _endorsementService.CreateEndorsementAsync(dto);
                        return StatusCode(201, ApiResponse<object>.SuccessResponse(
                    new { endorsementNumber = result.endorsement_number },
                    "Endorsement created successfully."));
        }


        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(long id)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(adminIdClaim, out long adminId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Admin ID missing from token."));
          
                var success = await _endorsementService.ApproveEndorsementAsync(id, adminId);
            if (!success)
                return NotFound(ApiResponse<object>.ErrorResponse("Endorsement not found."));

            return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Endorsement approved successfully."));
        }


        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(long id, [FromBody] RejectEndorsementDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return BadRequest(ApiResponse<object>.ErrorResponse("Rejection reason is mandatory."));

            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(adminIdClaim, out long adminId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Admin ID missing from token."));


            var success = await _endorsementService.RejectEndorsementAsync(id, adminId, dto.RejectionReason);

            if (!success)
                return NotFound(ApiResponse<object>.ErrorResponse("Endorsement not found."));

            return Ok(ApiResponse<object>.SuccessResponse(new { id }, "Endorsement rejected successfully."));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _endorsementService.GetEndorsementByIdAsync(id);
            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Endorsement not found."));

           
            return Ok(ApiResponse<object>.SuccessResponse(result, "Endorsement retrieved successfully."));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _endorsementService.GetAllPendingEndorsementsAsync();
            return Ok(ApiResponse<object>.SuccessResponse(result, "Endorsements retrieved successfully."));
        }
    }
}