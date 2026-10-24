using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System;
using System.Threading.Tasks;
using System.Security.Claims;

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

        // POST /api/v1/endorsements
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEndorsementDTO dto)
        {
            if (dto == null) return BadRequest("Invalid data.");

            try
            {
                var result = await _endorsementService.CreateEndorsementAsync(dto);
                return Ok(new { Message = "Endorsement Created", EndorsementNumber = result.endorsement_number });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // PUT /api/v1/endorsements/{id}/approve
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(long id)
        {
            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(adminIdClaim, out long adminId))
                return Unauthorized(new { Error = "Admin ID missing from token." });
            try
            {
                var success = await _endorsementService.ApproveEndorsementAsync(id, adminId);
                return success ? Ok(new { Message = "Endorsement Approved Successfully" }) : NotFound(new { Message = "Endorsement not found." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }

        // PUT /api/v1/endorsements/{id}/reject
        [HttpPut("{id}/reject")]
        public async Task<IActionResult> Reject(long id, [FromBody] RejectEndorsementDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RejectionReason))
                return BadRequest("Rejection reason is mandatory.");

            var adminIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(adminIdClaim, out long adminId))
                return Unauthorized(new { Error = "Admin ID missing from token." });

            var success = await _endorsementService.RejectEndorsementAsync(id, adminId, dto.RejectionReason);

            return success ? Ok(new { Message = "Endorsement Rejected Successfully" }) : NotFound(new { Message = "Endorsement not found." });
        }

        // GET /api/v1/endorsements/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(long id)
        {
            var result = await _endorsementService.GetEndorsementByIdAsync(id);
            return result != null ? Ok(result) : NotFound(new { Message = "Endorsement not found." });
        }
    }
}