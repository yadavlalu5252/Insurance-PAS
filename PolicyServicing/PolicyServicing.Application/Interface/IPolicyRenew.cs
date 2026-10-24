using PolicyServicing.Application.DTO;
using System.Threading.Tasks;

namespace PolicyServicing.Application.Interface
{
    public interface IPolicyRenew
    {
        Task<string> ProcessPolicyRenewalAsync(long customerId, long policyId, RenewalRequestDTO request);
        Task<RenewalDetailsResponseDTO> GetRenewalDetailsAsync(long policyId, long customerId);
    }
}