
using AgentManagement.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentManagement.Application.Interface
{
    public interface IAgentCommissionService
    {
        Task<AgentCommissionDTO2> CalculateCommission(AgentCommissionDTO dto);

        Task<List<AgentCommissionDTO2>> FetchCommissions(string? status, long? agentId, DateTime? fromDate, DateTime? toDate);

        Task<List<AgentCommissionDTO2>> FetchCommissionsByAgent(long agentId);

        Task UpdateCommissionStatus(long commissionId, string status);
    }
}
