
using AgentManagement.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AgentManagement.Application.Interface
{
    public interface ICommissionRuleService
    {
        Task AddRule(CommissionRuleDTO dto);

        Task<List<CommissionRuleDTO2>> FetchRules();

        Task<List<CommissionRuleDTO2>> FetchRulesByProduct(long productId);

        Task UpdateRule(long ruleId, CommissionRuleDTO dto);
    }
}
