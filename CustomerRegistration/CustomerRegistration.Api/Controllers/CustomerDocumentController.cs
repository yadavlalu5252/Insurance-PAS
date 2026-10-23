
using CustomerRegistration.Api.Wrappers;
using CustomerRegistration.Application.DTOs.Document;
using CustomerRegistration.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerRegistration.Api.Controllers;

[ApiController]
[Route("api/customers/{customerId:long}/documents")]
[Authorize]
public class CustomerDocumentController : ControllerBase
{
    private readonly ICustomerDocumentService service;

    public CustomerDocumentController(ICustomerDocumentService service)
    {
        this.service = service;
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Agent")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadDocument(long customerId, [FromForm] UploadCustomerDocumentDto dto)
    {
        var result = await service.UploadDocument(customerId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Customer not found."));

        return StatusCode(201, ApiResponse<CustomerDocumentDto>.SuccessResponse(result, "Document uploaded successfully."));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> GetDocuments(long customerId)
    {
        var result = await service.GetDocuments(customerId);

        return Ok(ApiResponse<List<CustomerDocumentDto>>.SuccessResponse(result, "Documents retrieved successfully."));
    }

    [HttpDelete("/api/customer-documents/{documentId:long}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> DeleteDocument(long documentId)
    {
        var result = await service.DeleteDocument(documentId);

        if (!result)
            return NotFound(ApiResponse<object>.ErrorResponse("Document not found."));

        return Ok(ApiResponse<object>.SuccessResponse(new { document_id = documentId }, "Document deleted successfully."));
    }

    [HttpPatch("/api/customer-documents/{documentId:long}/verify")]
    [Authorize(Roles = "Admin,Agent")]
    public async Task<IActionResult> VerifyDocument(long documentId, VerifyCustomerDocumentDto dto)
    {
        var result = await service.VerifyDocument(documentId, dto);

        if (result == null)
            return NotFound(ApiResponse<object>.ErrorResponse("Document not found."));

        return Ok(ApiResponse<CustomerDocumentDto>.SuccessResponse(result, "Document verification status updated successfully."));
    }
}
