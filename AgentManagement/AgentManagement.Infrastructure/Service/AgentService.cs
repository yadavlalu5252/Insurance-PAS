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
    public class AgentService : IAgentService
    {
        AppDbContext db;
        IMapper mapper;
        public AgentService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddAgent(AgentDTO dto)
        {
            var data = mapper.Map<Agent>(dto);

            data.CreatedAt = DateTime.UtcNow;

            await db.Agents.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<List<AgentDTO2>> FetchAllAgents()
        {
            var data = await db.Agents.ToListAsync();
            var ndata = mapper.Map<List<AgentDTO2>>(data);
            return ndata;
        }

        public async Task<AgentDTO2> FindAgentById(long id)
        {
            var data = await db.Agents.FindAsync(id);
            if(data == null)
            {
                throw new KeyNotFoundException("Agent Not found");
            }
            var ndata = mapper.Map<AgentDTO2>(data);
            return ndata;
        }


        public async Task UpdateAgent(long id, AgentDTO dto)
        {
            var data = await db.Agents.FindAsync(id);

            if (data == null)
                throw new KeyNotFoundException("Agent not found");

            mapper.Map(dto, data);

            data.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();
        }

    }
}
