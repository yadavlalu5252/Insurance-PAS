using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System;
using System.Threading.Tasks;

namespace PolicyServicing.API.Controllers
{
    [ApiController]
    [Route("api/v1")]
    public class CancellationController : ControllerBase
    {
        private readonly IProcessCancellation _cancellationService;

        public CancellationController(IProcessCancellation cancellationService)
        {
            _cancellationService = cancellationService;
        }

        // POST /api/v1/policies/{id}/cancel
        [HttpPost("policies/{id}/cancel")]
        public async Task<IActionResult> CancelPolicy(long id, [FromQuery] long customerId, [FromBody] CancellationRequestDTO request)
        {
            if (request == null)
                return BadRequest("Invalid cancellation payload.");

            try
            {
                var result = await _cancellationService.ProcessCancellationAsync(customerId, id, request);
                return Ok(new { Message = result });
            }
            catch (Exception ex)
            {
                // Catches FR-SRV-008 rules (Active claims, reason mandatory, etc.)
                return BadRequest(new { Error = ex.Message });
            }
        }

        // GET /api/v1/cancellations/{id}
        [HttpGet("cancellations/{id}")]
        public async Task<IActionResult> GetCancellationDetails(long id)
        {
            var result = await _cancellationService.GetCancellationDetailsAsync(id);
            if (result == null)
                return NotFound(new { Message = "Cancellation record not found." });

            return Ok(result);
        }
    }
}