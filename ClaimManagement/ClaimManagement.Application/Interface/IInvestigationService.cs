using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IInvestigationService
    {
        Task<InvestigationResponseDTO> CreateAsync(long claimId, CreateInvestigationDTO dto, long performedBy);
        Task<PagedResultDTO<InvestigationResponseDTO>> ListAsync(InvestigationFilterDTO filter);
        Task<InvestigationResponseDTO> GetByIdAsync(long id);
        Task<EvidenceResponseDTO> AddEvidenceAsync(long id, string? description, string? fileName, long fileLength, Stream? content, long performedBy);
        Task<List<EvidenceResponseDTO>> ListEvidenceAsync(long id);
        Task<InvestigationResponseDTO> SubmitReportAsync(long id, string? findings, string? recommendation, string? fileName, long fileLength, Stream? content, long performedBy);
    }
}
