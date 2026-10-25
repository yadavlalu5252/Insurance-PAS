using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/claims/{id}/documents")]
[ApiController]
public class ClaimDocumentsController : ControllerBase
{
    IClaimDocumentService service;
    ILogger<ClaimDocumentsController> logger;

    const long TempUserId = 0;

    public ClaimDocumentsController(IClaimDocumentService service, ILogger<ClaimDocumentsController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> UploadDocument(long id, [FromForm] string? document_name, IFormFile? file)
    {
        using var stream = file?.OpenReadStream();
        var d = await service.AddDocumentAsync(id, document_name, file?.FileName, file?.Length ?? 0, stream, TempUserId);
        logger.LogInformation("Document uploaded for claim {ClaimId}", id);
        return StatusCode(201, ApiResponse<DocumentResponseDTO>.Ok(d, "Document uploaded successfully"));
    }

    [HttpGet]
    [Route("required")]
    public async Task<IActionResult> RequiredDocuments(long id)
    {
        var d = await service.FetchRequiredAsync(id);
        return Ok(ApiResponse<List<string>>.Ok(d));
    }

    [HttpGet]
    public async Task<IActionResult> DocumentList(long id)
    {
        var d = await service.FetchDocumentsAsync(id);
        return Ok(ApiResponse<List<DocumentResponseDTO>>.Ok(d));
    }

    [HttpPut]
    [Route("verify")]
    public async Task<IActionResult> VerifyDocument(long id, VerifyDocumentDTO dto)
    {
        await service.VerifyDocumentAsync(id, dto, TempUserId);
        logger.LogInformation("Document {DocumentId} of claim {ClaimId} marked {Status}",
            dto.document_id, id, dto.verification_status);
        return Ok(ApiResponse<object>.Ok(null, "Document " + dto.verification_status + " successfully"));
    }
}