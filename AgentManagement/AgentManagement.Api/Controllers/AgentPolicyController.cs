
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagement.Api.Controllers
{
    [Route("api/v1/agents/{agentId}/policies")]
    [ApiController]
    public class AgentPolicyController : ControllerBase
    {
        IAgentPolicyService service;

        public AgentPolicyController(IAgentPolicyService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddPolicy(
            long agentId, AgentPolicyDTO dto)
        {
            await service.AddPolicy(agentId, dto);

            return Ok("Policy assigned successfully");
        }

        [HttpGet]
        public async Task<IActionResult> FetchPolicies(long agentId)
        {
            var data = await service.FetchPolicies(agentId);

            return Ok(data);
        }

        [HttpGet("{policyId}")]
        public async Task<IActionResult> FetchPolicy(
            long agentId, long policyId)
        {
            var data = await service.FetchPolicy(agentId, policyId);

            return Ok(data);
        }

        [HttpPatch("{policyId}/sold")]
        public async Task<IActionResult> MarkPolicyAsSold(long agentId, long policyId, [FromBody] DateTime saleDate)
        {
            await service.MarkPolicyAsSold(agentId, policyId, saleDate);
            return Ok("Policy marked as sold");
        }
    }
}
