
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using AgentManagement.Domain.Entities;
using AgentManagement.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AgentManagement.Infrastructure.Service
{
    public class AgentPolicyService : IAgentPolicyService
    {
        AppDbContext db;
        IMapper mapper;

        public AgentPolicyService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddPolicy(long agentId, AgentPolicyDTO dto)
        {
            var agent = await db.Agents.FindAsync(agentId);

            if (agent == null)
                throw new KeyNotFoundException("Agent not found");

            var data = mapper.Map<AgentPolicy>(dto);

            data.AgentId = agentId;
            data.AssignedDate = DateTime.UtcNow;
            data.CreatedAt = DateTime.UtcNow;
            data.Status = "Assigned";

            await db.AgentPolicies.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<List<AgentPolicyDTO2>> FetchPolicies(long agentId)
        {
            var data = await db.AgentPolicies
                .Where(x => x.AgentId == agentId)
                .ToListAsync();

            var ndata = mapper.Map<List<AgentPolicyDTO2>>(data);

            return ndata;
        }

        public async Task<AgentPolicyDTO2> FetchPolicy(
            long agentId, long policyId)
        {
            var data = await db.AgentPolicies
                .FirstOrDefaultAsync(x =>
                    x.AgentId == agentId &&
                    x.PolicyId == policyId);

            if (data == null)
                throw new KeyNotFoundException("Agent policy not found");

            var ndata = mapper.Map<AgentPolicyDTO2>(data);

            return ndata;
        }

        public async Task MarkPolicyAsSold( long agentId, long policyId, DateTime saleDate)
        {
            var policy = await db.AgentPolicies.FirstOrDefaultAsync(x =>
                x.AgentId == agentId && x.PolicyId == policyId);

            if (policy == null)
                throw new KeyNotFoundException("Agent policy not found");

            policy.Status = "Sold";
            policy.SaleDate = saleDate;

            await db.SaveChangesAsync();
        }
    }
}
