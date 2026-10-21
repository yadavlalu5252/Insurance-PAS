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
            var settlement = new Settlement
            {
                ClaimId = id,
                ApprovedAmount = dto.ApprovedAmount,
                PaymentMode = dto.PaymentMode,
                SettlementStatus = "Pending"
            };

            try
            {
                await service.Add(settlement);
                return Ok("Settlement saved successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/settlement")]
        public async Task<IActionResult> GetSettlement(long id)
        {
            var settlement = await service.GetByClaimId(id);

            if (settlement == null)
                return NotFound("Settlement not found");

            return Ok(settlement);
        }

        [HttpPut("{id}/settlement")]
        public async Task<IActionResult> UpdateSettlement(long id, SettlementDTO dto)
        {
            var settlement = await service.GetByClaimId(id);

            if (settlement == null)
                return NotFound("Settlement not found");

            settlement.ApprovedAmount = dto.ApprovedAmount;
            settlement.PaymentMode = dto.PaymentMode;
            settlement.SettlementStatus = dto.SettlementStatus;

            try
            {
                await service.Update(settlement);
                return Ok("Settlement updated successfully");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
