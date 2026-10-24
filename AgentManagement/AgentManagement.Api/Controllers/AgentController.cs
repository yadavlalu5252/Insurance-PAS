
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagement.Api.Controllers
{
    [Route("api/v1/agents")]
    [ApiController]
    public class AgentController : ControllerBase
    {
        IAgentService service;

        public AgentController(IAgentService service)
        {
            this.service = service;
        }

        [HttpPost]
        public async Task<IActionResult> AddAgent(AgentDTO dto)
        {
            await service.AddAgent(dto);
            return Ok("Agent added successfully");
        }

        [HttpGet]
        public async Task<IActionResult> FetchAllAgents()
        {
            var data = await service.FetchAllAgents();
            return Ok(data);
        }

        [HttpGet("{agentId}")]
        public async Task<IActionResult> FindAgentById(long agentId)
        {
            var data = await service.FindAgentById(agentId);
            return Ok(data);
        }

        [HttpPut("{agentId}")]
        public async Task<IActionResult> UpdateAgent(long agentId, AgentDTO dto)
        {
            await service.UpdateAgent(agentId, dto);
            return Ok("Agent updated successfully");
        }
    }
}
