
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Application.Interface
{
    public interface IPolicyEndorsementService
    {
        Task<PolicyEndorsementDTO?> CreateAsync(
            long policy_id, PolicyEndorsementDTO dto);

        Task<List<PolicyEndorsementDTO>> GetByPolicyIdAsync(
            long policy_id);

        Task<bool> ApproveAsync(
            long endorsement_id, string approved_by);

        Task<bool> RejectAsync(
            long endorsement_id, string approved_by);
    }
}
