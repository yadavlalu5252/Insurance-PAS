using PolicyServicing.Application.DTO;
using System.Threading.Tasks;

namespace PolicyServicing.Application.Interface
{
    public interface IProcessCancellation
    {
        Task<string> ProcessCancellationAsync(long customerId, long policyId, CancellationRequestDTO request);
        Task<CancellationDetailsDTO> GetCancellationDetailsAsync(long policyId, long customerId)
    }
}