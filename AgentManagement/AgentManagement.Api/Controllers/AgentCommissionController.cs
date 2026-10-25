using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagement.Api.Controllers
{
    [Route("api/v1/commissions")]
    [ApiController]
    public class AgentCommissionController : ControllerBase
    {
        IAgentCommissionService service;

        public AgentCommissionController(IAgentCommissionService service)
        {
            this.service = service;
        }

        [HttpPost("calculate")]
        public async Task<IActionResult> CalculateCommission(AgentCommissionDTO dto)
        {
            var data = await service.CalculateCommission(dto);
            return Ok(data);
        }

        [HttpGet]
        public async Task<IActionResult> FetchCommissions(
            [FromQuery] string? status,
            [FromQuery] long? agentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            var data = await service.FetchCommissions(
                status, agentId, fromDate, toDate);

            return Ok(data);
        }

        [HttpGet("/api/v1/agents/{agentId}/commissions")]
        public async Task<IActionResult> FetchCommissionsByAgent(long agentId)
        {
            var data = await service.FetchCommissionsByAgent(agentId);
            return Ok(data);
        }

        [HttpPatch("{commissionId}/status")]
        public async Task<IActionResult> UpdateCommissionStatus(
            long commissionId, [FromBody] string status)
        {
            await service.UpdateCommissionStatus(commissionId, status);
            return Ok("Commission status updated successfully");
        }
    }
}