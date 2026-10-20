using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IClaimService
    {
        Task<ClaimResponseDto> AddClaimAsync(ClaimCreateDto dto, long performedBy);

        Task<List<ClaimResponseDto>> FetchClaimsAsync();

        Task<ClaimResponseDto?> FetchClaimByIdAsync(long id);

        Task<ClaimStatusResponseDto?> FetchClaimStatusAsync(long id);

        // returns "Success", "NotFound" or "NotAllowed"
        Task<string> UpdateClaimAsync(long id, ClaimUpdateDto dto, long performedBy);
    }
}
