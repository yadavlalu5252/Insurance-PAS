using KycVerification.Api.Wrappers;
using KycVerification.Application.DTO;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KycVerifiaction.Api.Controllers
{
    [Route("api/v1/kyc/documents")]
    [ApiController]
    public class KycDocumentController : ControllerBase
    {
        private readonly IKycDocumentServices service;

        public KycDocumentController(IKycDocumentServices service)
        {
            this.service = service;
        }

        // GET: api/v1/kyc/documents/5
        [HttpGet("{documentId:int}")]
        public async Task<IActionResult> GetById(int documentId)
        {
            var data = await service.GetByIdAsync(documentId);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Document not found.",
                    null,
                    $"No document exists with ID {documentId}."
                ));
            }

            return Ok(new ApiResponse<KycDocument>(
                true,
                "Document retrieved successfully.",
                data
            ));
        }

        // GET: api/v1/kyc/documents/kyc/5
        [HttpGet("kyc/{kycId:int}")]
        public async Task<IActionResult> GetByKycId(int kycId)
        {
            var data = await service.GetByKycIdAsync(kycId);

            return Ok(new ApiResponse<List<KycDocument>>(
                true,
                "KYC documents retrieved successfully.",
                data
            ));
        }

        // POST: api/v1/kyc/documents
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] KycDocument document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid document data.",
                    null,
                    "Please check the submitted fields."
                ));
            }

            var saved = await service.AddAsync(document);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<KycDocument>(
                    true,
                    "Document record created successfully.",
                    saved
                )
            );
        }

        // PUT: api/v1/kyc/documents/5
        [HttpPut("{documentId:int}")]
        public async Task<IActionResult> Update( int documentId,[FromBody] KycDocument document)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid document data.",
                    null,
                    "Please check the submitted fields."
                ));
            }

            if (documentId != document.DocumentId)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Document ID mismatch.",
                    null,
                    "The route ID must match the document ID in the request body."
                ));
            }

            var existing = await service.GetByIdAsync(documentId);

            if (existing == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Document not found.",
                    null,
                    $"No document exists with ID {documentId}."
                ));
            }

            await service.UpdateAsync(document);

            return Ok(new ApiResponse<object>(
                true,
                "Document updated successfully.",
                null
            ));
        }



        [HttpPost("upload")]
        public async Task<IActionResult> Upload( [FromForm] UploadKycDocumentDto dto)
        {
            try
            {
                var saved = await service.UploadAsync(
                    dto.KycId,
                    dto.DocumentType,
                    dto.DocumentNumber,
                    dto.File);

                return StatusCode(201,
                    new ApiResponse<KycDocument>(
                        true,
                        "Document uploaded successfully.",
                        saved));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(
                    false, ex.Message, null, ex.Message));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ApiResponse<object>(
                    false, ex.Message, null, ex.Message));
            }
        }

    }
}
