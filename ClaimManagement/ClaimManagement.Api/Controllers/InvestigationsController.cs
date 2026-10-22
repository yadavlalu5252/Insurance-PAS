using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[ApiController]
public class InvestigationsController : ControllerBase
{
    IInvestigationService service;
    ILogger<InvestigationsController> logger;

    // Temporary user id until JWT login is added
    const long TempUserId = 0;

    public InvestigationsController(IInvestigationService service, ILogger<InvestigationsController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    // BRD: Assign Investigator.  Admin only once JWT is added
    [HttpPost]
    [Route("claims/{claimId}/investigation")]
    public async Task<IActionResult> CreateInvestigation(long claimId, CreateInvestigationDTO dto)
    {
        var d = await service.CreateAsync(claimId, dto, TempUserId);
        logger.LogInformation("Investigation {Number} created for claim {ClaimId}", d.investigation_number, claimId);
        return StatusCode(201, ApiResponse<InvestigationResponseDTO>.Ok(d, "Investigation created successfully"));
    }

    // Example: GET /api/v1/investigations?claim_id=1&investigation_status=Pending
    [HttpGet]
    [Route("investigations")]
    public async Task<IActionResult> InvestigationList([FromQuery] InvestigationFilterDTO filter)
    {
        var d = await service.ListAsync(filter);
        return Ok(ApiResponse<PagedResultDTO<InvestigationResponseDTO>>.Ok(d));
    }

    [HttpGet]
    [Route("investigations/{id}")]
    public async Task<IActionResult> InvestigationById(long id)
    {
        var d = await service.GetByIdAsync(id);
        return Ok(ApiResponse<InvestigationResponseDTO>.Ok(d));
    }

    [HttpPost]
    [Route("investigations/{id}/evidence")]
    public async Task<IActionResult> AddEvidence(long id, [FromForm] string? description, IFormFile? file)
    {
        using var stream = file?.OpenReadStream();
        var d = await service.AddEvidenceAsync(id, description, file?.FileName, file?.Length ?? 0, stream, TempUserId);
        logger.LogInformation("Evidence added to investigation {InvestigationId}", id);
        return StatusCode(201, ApiResponse<EvidenceResponseDTO>.Ok(d, "Evidence added successfully"));
    }

    [HttpGet]
    [Route("investigations/{id}/evidence")]
    public async Task<IActionResult> EvidenceList(long id)
    {
        var d = await service.ListEvidenceAsync(id);
        return Ok(ApiResponse<List<EvidenceResponseDTO>>.Ok(d));
    }

    // BRD: Submit Investigation Report
    [HttpPut]
    [Route("investigations/{id}")]
    public async Task<IActionResult> SubmitReport(long id, [FromForm] string? findings,
        [FromForm] string? recommendation, IFormFile? file)
    {
        using var stream = file?.OpenReadStream();
        var d = await service.SubmitReportAsync(id, findings, recommendation, file?.FileName, file?.Length ?? 0, stream, TempUserId);
        logger.LogInformation("Report submitted for investigation {InvestigationId}", id);
        return Ok(ApiResponse<InvestigationResponseDTO>.Ok(d, "Investigation report submitted successfully"));
    }
}