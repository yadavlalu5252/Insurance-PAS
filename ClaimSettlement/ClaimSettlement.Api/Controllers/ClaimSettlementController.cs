
using ClaimSettlement.Application.DTO;
using ClaimSettlement.Application.Interfaces;
using ClaimSettlement.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ClaimSettlement.Api.Controllers
{
    [Route("api/v1/claims")]
    [ApiController]
    public class ClaimSettlementController : ControllerBase
    {
        private readonly IClaimSettlementService service;

        public ClaimSettlementController(IClaimSettlementService service)
        {
            this.service = service;
        }

        [HttpPost("{id}/settlement")]
        public async Task<IActionResult> ProcessSettlement(long id, SettlementDTO dto)
        {
            if (dto.ApprovedAmount <= 0)
                return BadRequest("Approved amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(dto.PaymentMode))
                return BadRequest("Payment mode is required");

            var settlement = new Settlement
            {
                ClaimId = id,
                ApprovedAmount = dto.ApprovedAmount,
                PaymentMode = dto.PaymentMode,
                SettlementStatus = "Pending"
            };

            await service.Add(settlement);

            return Ok("Settlement saved successfully");
        }

        [HttpGet("{id}/settlement")]
        public async Task<IActionResult> GetSettlement(long id)
        {
            var settlement = await service.GetByClaimId(id);

            if (settlement == null)
                return NotFound("Settlement not found");

            return Ok(settlement);
        }
    }
}
