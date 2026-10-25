using AgentManagement.Application.DTO;
using AgentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Application.Interface
{
    public interface IAgentService
    {
        Task AddAgent(AgentDTO dto);

        Task<AgentDTO2> FindAgentById(long id);

        Task<List<AgentDTO2>> FetchAllAgents();

        Task UpdateAgent(long id, AgentDTO dto);
    }
}
