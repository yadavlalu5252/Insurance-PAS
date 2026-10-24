
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagement.Api.Controllers
{
    [Route("api/v1/agents/{agentId}/customers")]
    [ApiController]
    public class AgentCustomerController : ControllerBase
    {
        IAgentCustomerService service;

        public AgentCustomerController(IAgentCustomerService service)
        {
            this.service = service;
        }

        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok("AgentCustomerController is working");
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer(
            long agentId, AgentCustomerDTO dto)
        {
            await service.AddCustomer(agentId, dto);
            return Ok("Customer assigned successfully");
        }

        [HttpGet]
        public async Task<IActionResult> FetchCustomers(long agentId)
        {
            var data = await service.FetchCustomers(agentId);
            return Ok(data);
        }



        [HttpPut("{customerId}/transfer")]
        public async Task<IActionResult> TransferCustomer(
            long agentId,
            long customerId,
            long newAgentId,
            long assignedBy)
        {
            await service.TransferCustomer(
                agentId,
                customerId,
                newAgentId,
                assignedBy);

            return Ok("Customer transferred successfully");
        }

    }
}
