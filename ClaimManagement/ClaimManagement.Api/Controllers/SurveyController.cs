using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}")]
[ApiController]
public class SurveyController : ControllerBase
{
    ISurveyService service;
    ILogger<SurveyController> logger;

    const long TempUserId = 0;

    public SurveyController(ISurveyService service, ILogger<SurveyController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    // Admin only once JWT is added
    [HttpPost]
    [Route("claims/{id}/survey")]
    public async Task<IActionResult> AssignSurveyor(long id, AssignSurveyDTO dto)
    {
        var d = await service.AssignSurveyorAsync(id, dto, TempUserId);
        logger.LogInformation("Surveyor assigned to claim {ClaimId}", id);
        return StatusCode(201, ApiResponse<SurveyResponseDTO>.Ok(d, "Surveyor assigned successfully"));
    }

    [HttpGet]
    [Route("claims/{id}/survey")]
    public async Task<IActionResult> ViewSurvey(long id)
    {
        var d = await service.FetchSurveyAsync(id);
        return Ok(ApiResponse<SurveyResponseDTO>.Ok(d));
    }

    [HttpPut]
    [Route("surveys/{surveyId}")]
    public async Task<IActionResult> SubmitSurveyReport(long surveyId, [FromForm] DateTime inspection_date, IFormFile? file)
    {
        using var stream = file?.OpenReadStream();
        await service.SubmitReportAsync(surveyId, inspection_date, file?.FileName, file?.Length ?? 0, stream, TempUserId);
        logger.LogInformation("Survey {SurveyId} report submitted", surveyId);
        return Ok(ApiResponse<object>.Ok(null, "Survey report submitted successfully"));
    }
}