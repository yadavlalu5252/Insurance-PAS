using PolicyServicing.Application.DTO;
using PolicyServicing.Domain.Entities;
using System.Threading.Tasks;

namespace PolicyServicing.Application.Interface
{
    public interface IEndorsementService
    {
        Task<policy_endorsement> CreateEndorsementAsync(CreateEndorsementDTO dto);
        Task<bool> ApproveEndorsementAsync(long endorsementId, long adminId);
        Task<bool> RejectEndorsementAsync(long endorsementId, long adminId, string reason);
        Task<policy_endorsement?> GetEndorsementByIdAsync(long endorsementId);
    }
}