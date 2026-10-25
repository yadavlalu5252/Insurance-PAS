using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using AgentManagement.Domain.Entities;
using AgentManagement.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace AgentManagement.Infrastructure.Service
{
    public class AgentCustomerService : IAgentCustomerService
    {
        AppDbContext db;
        IMapper mapper;

        public AgentCustomerService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddCustomer(long agentId, AgentCustomerDTO dto)
        {
            var agent = await db.Agents.FindAsync(agentId);

            if (agent == null)
                throw new KeyNotFoundException("Agent not found");

            var data = mapper.Map<AgentCustomer>(dto);

            data.AgentId = agentId;
            data.AssignedDate = DateTime.UtcNow;
            data.CreatedAt = DateTime.UtcNow;
            data.Status = "Active";

            await db.AgentCustomers.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<List<AgentCustomerDTO2>> FetchCustomers(long agentId)
        {
            var data = await db.AgentCustomers
                .Where(x => x.AgentId == agentId)
                .ToListAsync();

            var ndata = mapper.Map<List<AgentCustomerDTO2>>(data);

            return ndata;
        }

        public async Task TransferCustomer(
    long agentId,
    long customerId,
    long newAgentId,
    long assignedBy)
        {
            var oldAssignment = await db.AgentCustomers
                .FirstOrDefaultAsync(x =>
                    x.AgentId == agentId &&
                    x.CustomerId == customerId &&
                    x.Status == "Active");

            if (oldAssignment == null)
                throw new KeyNotFoundException("Active customer assignment not found");

            var newAgent = await db.Agents.FindAsync(newAgentId);

            if (newAgent == null)
                throw new KeyNotFoundException("New agent not found");

            oldAssignment.Status = "Transferred";

            var newAssignment = new AgentCustomer
            {
                AgentId = newAgentId,
                CustomerId = customerId,
                AssignedDate = DateTime.UtcNow,
                AssignedBy = assignedBy,
                Status = "Active",
                CreatedAt = DateTime.UtcNow
            };

            await db.AgentCustomers.AddAsync(newAssignment);
            await db.SaveChangesAsync();
        }
    }
}
