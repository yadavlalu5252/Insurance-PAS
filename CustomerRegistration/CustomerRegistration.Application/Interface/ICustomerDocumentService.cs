
using CustomerRegistration.Application.DTOs.Document;

namespace CustomerRegistration.Application.Interfaces;

public interface ICustomerDocumentService
{
    Task<CustomerDocumentDto?> UploadDocument(long customerId, UploadCustomerDocumentDto dto);
    Task<List<CustomerDocumentDto>> GetDocuments(long customerId);
    Task<bool> DeleteDocument(long documentId);
    Task<CustomerDocumentDto?> VerifyDocument(long documentId, VerifyCustomerDocumentDto dto);
}
