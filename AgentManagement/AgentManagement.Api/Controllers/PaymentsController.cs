
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagement.Api.Controllers
{
    [ApiController]
    [Route("api/v1/payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly IAgentCommissionPaymentService service;

        public PaymentsController(
            IAgentCommissionPaymentService service)
        {
            this.service = service;
        }

        // Create a payout
        [HttpPost("payouts")]
        public async Task<IActionResult> CreatePayout(
            [FromBody] AgentCommissionPaymentDTO dto)
        {
            var result = await service.CreatePayout(dto);
            return Ok(result);
        }

        // Get all payouts with optional filters
        [HttpGet("payouts")]
        public async Task<IActionResult> FetchPayouts(
            [FromQuery] string? paymentMode,
            [FromQuery] string? status)
        {
            var result = await service.FetchPayouts(
                paymentMode, status);

            return Ok(result);
        }

        // Reconcile a payment
        [HttpPut("payouts/{paymentId}/reconcile")]
        public async Task<IActionResult> ReconcilePayment(
            long paymentId,
            [FromBody] ReconcilePaymentDTO dto)
        {
            await service.ReconcilePayment(
                paymentId,
                dto.TransactionReference,
                dto.Status);

            return Ok("Payment reconciled successfully");
        }

        // Get payout history for a specific agent
        [HttpGet("/api/v1/agents/{agentId}/payments/payouts")]
        public async Task<IActionResult> FetchAgentPayouts(
            long agentId)
        {
            var result = await service.FetchPayoutsByAgent(agentId);
            return Ok(result);
        }
    }
}
