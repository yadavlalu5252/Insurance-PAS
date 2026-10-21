
using PolicyIssuance.Application.DTOs;

namespace PolicyIssuance.Application.Interface
{
    public interface IPolicyClosureService
    {
        Task AddClosure(PolicyClosureDTO dto);

        Task<PolicyClosureDTO2?> FindClosureById(long id);

        Task<List<PolicyClosureDTO2>> FetchAllClosure();

        Task<List<PolicyClosureDTO2>> FetchPendingClosure();

        Task<bool> ApproveClosure(
            long id, string approvedBy, decimal? refundAmount);

        Task<bool> RejectClosure(
            long id, string rejectionReason, string rejectedBy);
    }
}
