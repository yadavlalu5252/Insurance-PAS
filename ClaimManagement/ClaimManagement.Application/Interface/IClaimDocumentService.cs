using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IClaimDocumentService
    {
        Task<DocumentResponseDTO> AddDocumentAsync(long claimId, string? documentName,
            string? fileName, long fileLength, Stream? content, long performedBy);

        Task<List<DocumentResponseDTO>> FetchDocumentsAsync(long claimId);

        Task VerifyDocumentAsync(long claimId, VerifyDocumentDTO dto, long performedBy);
    }
}
