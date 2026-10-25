using AgentManagement.Application.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.Interface
{
    public interface IAgentCustomerService
    {
        Task AddCustomer(long agentId, AgentCustomerDTO dto);

        Task<List<AgentCustomerDTO2>> FetchCustomers(long agentId);

        Task TransferCustomer(
            long agentId,
            long customerId,
            long newAgentId,
            long assignedBy);
    }
}
