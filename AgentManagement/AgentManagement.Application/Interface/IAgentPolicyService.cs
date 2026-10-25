
using AgentManagement.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentManagement.Application.Interface
{
    public interface IAgentPolicyService
    {
        Task AddPolicy(long agentId, AgentPolicyDTO dto);

        Task<List<AgentPolicyDTO2>> FetchPolicies(long agentId);

        Task<AgentPolicyDTO2> FetchPolicy(long agentId, long policyId);

        Task MarkPolicyAsSold(long agentId, long policyId, DateTime saleDate);
    }
}
