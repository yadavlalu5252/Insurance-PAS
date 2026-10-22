using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/claims/{id}/assessment")]
[ApiController]
public class AssessmentController : ControllerBase
{
    IAssessmentService service;
    ILogger<AssessmentController> logger;

    const long TempUserId = 0;

    public AssessmentController(IAssessmentService service, ILogger<AssessmentController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    // Admin only once JWT is added
    [HttpPost]
    public async Task<IActionResult> SaveAssessment(long id, CreateAssessmentDTO dto)
    {
        var d = await service.SaveAssessmentAsync(id, dto, TempUserId);
        logger.LogInformation("Claim {ClaimId} assessed", id);
        return Ok(ApiResponse<AssessmentResponseDTO>.Ok(d, "Assessment saved successfully"));
    }

    [HttpGet]
    public async Task<IActionResult> ViewAssessment(long id)
    {
        var d = await service.FetchAssessmentAsync(id);
        return Ok(ApiResponse<AssessmentResponseDTO>.Ok(d));
    }
}