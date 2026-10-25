
using PremiumSchedule.Application.DTO;

namespace PremiumSchedule.Application.Interfaces
{
    public interface IPolicyIssuanceClient
    {
        Task<PolicyDto?> GetPolicyAsync(int policyId);

        Task<string?> GetPremiumFrequency(int policyId);
    }
}