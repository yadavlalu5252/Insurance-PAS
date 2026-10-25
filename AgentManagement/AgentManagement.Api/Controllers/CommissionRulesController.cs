
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using Microsoft.AspNetCore.Mvc;

namespace AgentManagement.API.Controllers
{
    [ApiController]
    [Route("api/v1/commission-rules")]
    public class CommissionRulesController : ControllerBase
    {
        private readonly ICommissionRuleService service;

        public CommissionRulesController(
            ICommissionRuleService service)
        {
            this.service = service;
        }

        // POST /api/v1/commission-rules
        [HttpPost]
        public async Task<IActionResult> AddRule(
            [FromBody] CommissionRuleDTO dto)
        {
            await service.AddRule(dto);

            return Ok(new
            {
                message = "Commission rule created successfully"
            });
        }

        // GET /api/v1/commission-rules
        [HttpGet]
        public async Task<IActionResult> FetchRules()
        {
            var data = await service.FetchRules();

            return Ok(data);
        }

        // GET /api/v1/commission-rules/product/{productId}
        [HttpGet("product/{productId:long}")]
        public async Task<IActionResult> FetchRulesByProduct(
            long productId)
        {
            var data = await service.FetchRulesByProduct(productId);

            return Ok(data);
        }

        // PUT /api/v1/commission-rules/{ruleId}
        [HttpPut("{ruleId:long}")]
        public async Task<IActionResult> UpdateRule(
            long ruleId,
            [FromBody] CommissionRuleDTO dto)
        {
            await service.UpdateRule(ruleId, dto);

            return Ok(new
            {
                message = "Commission rule updated successfully"
            });
        }
    }
}
