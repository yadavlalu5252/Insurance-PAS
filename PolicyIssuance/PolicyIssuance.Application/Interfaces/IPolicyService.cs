
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Application.Interface
{
    public interface IPolicyService
    {
        Task<List<PolicyResponseDTO>> GetAllAsync();

        Task<PolicyResponseDTO?> GetByIdAsync(long id);

        Task<PolicyResponseDTO?> CreateAsync(PolicyDTO dto);

        Task<PolicyResponseDTO?> UpdateAsync(long id, PolicyDTO dto);

        Task<bool> DeleteAsync(long id);

        Task<bool> ActivateAsync(long id, string performed_by);

        Task<bool> SuspendAsync(long id, string performed_by);

        Task<bool> CancelAsync(long id, string performed_by);

        Task<bool> ExpireAsync(long id, string performed_by);

        Task<List<PolicyCoverageDTO>> GetCoveragesAsync(long policyId);

        Task<PolicyCoverageDTO?> UpdateCoverageAsync(long policyId, long coverageId, PolicyCoverageDTO dto);

        Task<bool> UpdateRidersAsync(long policyId, List<PolicyRiderDTO> riders);
        Task<List<PolicyRiderDTO>> GetRidersAsync(long policyId);
    }
}
