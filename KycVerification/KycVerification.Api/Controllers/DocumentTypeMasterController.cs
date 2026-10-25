using KycVerification.Api.Wrappers;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KycVerifiaction.Api.Controllers
{
    [Route("api/v1/kyc/document-types")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DocumentTypeMasterController : ControllerBase
    {
        private readonly IDocumentTypeMasterServices service;

        public DocumentTypeMasterController(
            IDocumentTypeMasterServices service)
        {
            this.service = service;
        }

        // GET: api/v1/kyc/document-types
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var data = await service.GetAllAsync();

            return Ok(new ApiResponse<List<DocumentTypeMaster>>(
                true,
                "Document types retrieved successfully.",
                data
            ));
        }

        // GET: api/v1/kyc/document-types/1
        [HttpGet("{documentTypeId:int}")]
        public async Task<IActionResult> GetById(int documentTypeId)
        {
            var data = await service.GetByIdAsync(documentTypeId);

            if (data == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Document type not found.",
                    null,
                    $"No document type exists with ID {documentTypeId}."
                ));
            }

            return Ok(new ApiResponse<DocumentTypeMaster>(
                true,
                "Document type retrieved successfully.",
                data
            ));
        }

        // POST: api/v1/kyc/document-types
        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] DocumentTypeMaster documentType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid document type data.",
                    null,
                    "Please check the submitted fields."
                ));
            }

            documentType.DocumentTypeId = 0;

            var saved = await service.AddAsync(documentType);

            return StatusCode(
                StatusCodes.Status201Created,
                new ApiResponse<DocumentTypeMaster>(
                    true,
                    "Document type created successfully.",
                    saved
                )
            );
        }

        // PUT: api/v1/kyc/document-types/1
        [HttpPut("{documentTypeId:int}")]
        public async Task<IActionResult> Update(
            int documentTypeId,
            [FromBody] DocumentTypeMaster documentType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Invalid document type data.",
                    null,
                    "Please check the submitted fields."
                ));
            }

            if (documentTypeId != documentType.DocumentTypeId)
            {
                return BadRequest(new ApiResponse<object>(
                    false,
                    "Document type ID mismatch.",
                    null,
                    "The route ID must match the ID in the request body."
                ));
            }

            var existing = await service.GetByIdAsync(documentTypeId);

            if (existing == null)
            {
                return NotFound(new ApiResponse<object>(
                    false,
                    "Document type not found.",
                    null,
                    $"No document type exists with ID {documentTypeId}."
                ));
            }

            await service.UpdateAsync(documentType);

            return Ok(new ApiResponse<DocumentTypeMaster>(
                true,
                "Document type updated successfully.",
                documentType
            ));
        }
    }
}