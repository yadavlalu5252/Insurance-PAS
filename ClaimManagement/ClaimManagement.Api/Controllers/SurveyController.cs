using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}")]
    [ApiController]
    public class SurveyController : ControllerBase
    {
        ISurveyService service;
        ILogger<SurveyController> logger;

        // Temporary user id until JWT login is added
        const long TempUserId = 0;

        static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };

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
            if (dto.surveyor_id <= 0)
                return BadRequest(ApiResponse<object>.Fail("Surveyor id is required"));

            var (result, survey) = await service.AssignSurveyorAsync(id, dto, TempUserId);

            if (result == "ClaimNotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "AlreadyAssigned")
                return Conflict(ApiResponse<object>.Fail("A surveyor is already assigned to this claim"));

            if (result == "InvalidStatus")
                return Conflict(ApiResponse<object>.Fail("A surveyor can be assigned only after documents are submitted (Pending Verification)"));

            logger.LogInformation("Surveyor {SurveyorId} assigned to claim {ClaimId}", dto.surveyor_id, id);

            return StatusCode(201, ApiResponse<SurveyResponseDTO>.Ok(survey!, "Surveyor assigned successfully"));
        }

        [HttpGet]
        [Route("claims/{id}/survey")]
        public async Task<IActionResult> ViewSurvey(long id)
        {
            var d = await service.FetchSurveyByClaimAsync(id);

            if (d == null)
                return NotFound(ApiResponse<object>.Fail("No survey found for this claim"));

            return Ok(ApiResponse<SurveyResponseDTO>.Ok(d));
        }

        // Surveyor or Admin once JWT is added
        [HttpPut]
        [Route("surveys/{surveyId}")]
        public async Task<IActionResult> SubmitReport(long surveyId, [FromForm] DateTime inspection_date, [FromForm] IFormFile report_file)
        {
            if (report_file == null || report_file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("Report file is required"));

            var ext = Path.GetExtension(report_file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return BadRequest(ApiResponse<object>.Fail("Only pdf, jpg, jpeg and png files are allowed"));

            if (inspection_date.Date > DateTime.Now.Date)
                return BadRequest(ApiResponse<object>.Fail("Inspection date cannot be in the future"));

            using var stream = report_file.OpenReadStream();
            var result = await service.SubmitReportAsync(surveyId, inspection_date, ext, stream, TempUserId);

            if (result == "NotFound")
                return NotFound(ApiResponse<object>.Fail("Survey not found"));

            if (result == "AlreadyCompleted")
                return Conflict(ApiResponse<object>.Fail("The report for this survey is already submitted"));

            if (result == "InvalidStatus")
                return Conflict(ApiResponse<object>.Fail("The claim is no longer under survey"));

            if (result == "InvalidDate")
                return BadRequest(ApiResponse<object>.Fail("Inspection date cannot be before the assigned date"));

            logger.LogInformation("Survey {SurveyId} report submitted", surveyId);

            return Ok(ApiResponse<object>.Ok(null, "Survey report submitted successfully"));
        }
    }
}
