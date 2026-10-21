
using ClaimSettlement.Domain.Entities;

namespace ClaimSettlement.Application.Interfaces
{
    public interface IClaimSettlementService
    {
        Task<Settlement?> GetByClaimId(long claimId);

        Task Add(Settlement s);

        Task Update(Settlement s);
    }
}
