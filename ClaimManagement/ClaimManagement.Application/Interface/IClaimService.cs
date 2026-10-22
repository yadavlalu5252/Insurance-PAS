using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IClaimService
    {
        Task<ClaimResponseDTO> AddClaimAsync(CreateClaimDTO dto, long performedBy);
        Task<PagedResultDTO<ClaimResponseDTO>> FetchClaimsAsync(ClaimFilterDTO filter);
        Task<ClaimResponseDTO> FetchClaimByIdAsync(long id);
        Task<ClaimStatusDTO> FetchClaimStatusAsync(long id);
        Task<List<ClaimHistoryDTO>> FetchClaimHistoryAsync(long id);
        Task UpdateClaimAsync(long id, UpdateClaimDTO dto, long performedBy);
    }
}
