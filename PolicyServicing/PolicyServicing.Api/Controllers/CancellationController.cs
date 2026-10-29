using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using PolicyServicing.Api.Wrappers;

namespace PolicyServicing.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1")]
    public class CancellationController : ControllerBase
    {
        private readonly IProcessCancellation _cancellationService;

        public CancellationController(IProcessCancellation cancellationService)
        {
            _cancellationService = cancellationService;
        }


        [HttpPost("policies/{id}/cancel")]
        public async Task<IActionResult> CancelPolicy(long id, [FromBody] CancellationRequestDTO request)
        {
            if (request == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Cancellation record not found or unauthorized."));



            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out long customerId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));



            var result = await _cancellationService.ProcessCancellationAsync(customerId, id, request);
            return Ok(ApiResponse<string>.SuccessResponse(result, "Policy cancellation processed successfully."));

        }


        [HttpGet("cancellations/{id}")]
        public async Task<IActionResult> GetCancellationDetails(long id)
        {

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!long.TryParse(userIdClaim, out long customerId))
                return Unauthorized(ApiResponse<object>.ErrorResponse("Customer ID missing from token."));


            var result = await _cancellationService.GetCancellationDetailsAsync(id, customerId);

            if (result == null)
                return NotFound(ApiResponse<object>.ErrorResponse("Cancellation record not found or unauthorized."));

            return Ok(ApiResponse<CancellationDetailsDTO>.SuccessResponse(result, "Cancellation details retrieved successfully."));
        }
    }
}