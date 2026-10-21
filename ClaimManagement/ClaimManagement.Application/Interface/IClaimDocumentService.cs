using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IClaimDocumentService
    {
        // result is "Success", "NotFound" or "NotAllowed"
        Task<(string result, DocumentResponseDTO? data)> AddDocumentAsync(
            long claimId, string documentName, string extension, Stream content, long performedBy);

        // returns null if the claim does not exist
        Task<List<DocumentResponseDTO>?> FetchDocumentsAsync(long claimId);

        // result is "Success", "ClaimNotFound" or "DocumentNotFound"
        Task<string> VerifyDocumentAsync(long claimId, VerifyDocumentDTO dto, long performedBy);
    }
}
