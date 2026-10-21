using Asp.Versioning;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Application.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace ClaimManagement.Api.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/claims/{id}/documents")]
    [ApiController]
    public class ClaimDocumentsController : ControllerBase
    {
        IClaimDocumentService service;
        ILogger<ClaimDocumentsController> logger;

        // Temporary user id until JWT login is added
        const long TempUserId = 0;

        static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        const long MaxFileSize = 5 * 1024 * 1024; // 5 MB

        public ClaimDocumentsController(IClaimDocumentService service, ILogger<ClaimDocumentsController> logger)
        {
            this.service = service;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> UploadDocument(long id, [FromForm] string document_name, IFormFile file)
        {
            if (string.IsNullOrWhiteSpace(document_name) || document_name.Length > 150)
                return BadRequest(ApiResponse<object>.Fail("Document name is required (max 150 characters)"));

            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.Fail("File is required"));

            if (file.Length > MaxFileSize)
                return BadRequest(ApiResponse<object>.Fail("File size cannot be more than 5 MB"));

            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                return BadRequest(ApiResponse<object>.Fail("Only pdf, jpg, jpeg and png files are allowed"));

            using var stream = file.OpenReadStream();
            var (result, data) = await service.AddDocumentAsync(id, document_name, extension, stream, TempUserId);

            if (result == "NotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "NotAllowed")
                return Conflict(ApiResponse<object>.Fail("Documents cannot be added in the current claim status"));

            logger.LogInformation("Document uploaded for claim {ClaimId}", id);

            return StatusCode(201, ApiResponse<DocumentResponseDTO>.Ok(data, "Document uploaded successfully"));
        }

        [HttpGet]
        public async Task<IActionResult> DocumentList(long id)
        {
            var d = await service.FetchDocumentsAsync(id);

            if (d == null)
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            return Ok(ApiResponse<List<DocumentResponseDTO>>.Ok(d));
        }

        [HttpPut]
        [Route("verify")]
        public async Task<IActionResult> VerifyDocument(long id, VerifyDocumentDTO dto)
        {
            if (dto.document_id <= 0)
                return BadRequest(ApiResponse<object>.Fail("Document id is required"));

            if (dto.verification_status != "Verified" && dto.verification_status != "Rejected")
                return BadRequest(ApiResponse<object>.Fail("Verification status must be Verified or Rejected"));

            var result = await service.VerifyDocumentAsync(id, dto, TempUserId);

            if (result == "ClaimNotFound")
                return NotFound(ApiResponse<object>.Fail("Claim not found"));

            if (result == "DocumentNotFound")
                return NotFound(ApiResponse<object>.Fail("Document not found for this claim"));

            logger.LogInformation("Document {DocumentId} of claim {ClaimId} marked {Status}",
                dto.document_id, id, dto.verification_status);

            return Ok(ApiResponse<object>.Ok(null, "Document " + dto.verification_status + " successfully"));
        }
    }
}
