using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/claims/{id}/settlement")]
[ApiController]
public class SettlementController : ControllerBase
{
    ISettlementService service;
    ILogger<SettlementController> logger;

    // Temporary user id until JWT login is added
    const long TempUserId = 0;

    public SettlementController(ISettlementService service, ILogger<SettlementController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    // BRD: Process Settlement.  Admin only once JWT is added
    [HttpPost]
    public async Task<IActionResult> ProcessSettlement(long id, ProcessSettlementDTO dto)
    {
        var d = await service.ProcessAsync(id, dto, TempUserId);
        logger.LogInformation("Claim {ClaimId} settled", id);
        return StatusCode(201, ApiResponse<SettlementResponseDTO>.Ok(d, "Settlement recorded successfully"));
    }

    // BRD: View Settlement
    [HttpGet]
    public async Task<IActionResult> ViewSettlement(long id)
    {
        var d = await service.GetAsync(id);
        return Ok(ApiResponse<SettlementResponseDTO>.Ok(d));
    }

    // my addition: settlement advice
    [HttpGet]
    [Route("advice")]
    public async Task<IActionResult> SettlementAdvice(long id)
    {
        var d = await service.GetAdviceAsync(id);
        return Ok(ApiResponse<SettlementAdviceDTO>.Ok(d));
    }
}