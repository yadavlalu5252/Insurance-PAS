using ClaimManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface ISettlementService
    {
        Task<SettlementResponseDTO> ProcessAsync(long claimId, ProcessSettlementDTO dto, long performedBy);
        Task<SettlementResponseDTO> GetAsync(long claimId);
        Task<SettlementAdviceDTO> GetAdviceAsync(long claimId);
    }
}
