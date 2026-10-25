using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using AgentManagement.Domain.Entities;
using AgentManagement.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AgentManagement.Infrastructure.Service
{
    public class AgentCommissionService : IAgentCommissionService
    {
        AppDbContext db;
        IMapper mapper;

        public AgentCommissionService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<AgentCommissionDTO2> CalculateCommission(
            AgentCommissionDTO dto)
        {
            if (dto.AgentId <= 0 || dto.PolicyId <= 0 || dto.ProductId <= 0)
                throw new ArgumentException("AgentId, PolicyId and ProductId must be valid");

            if (dto.PremiumAmount <= 0)
                throw new ArgumentException("PremiumAmount must be greater than zero");

            var agent = await db.Agents.FindAsync(dto.AgentId);

            if (agent == null)
                throw new KeyNotFoundException("Agent not found");

            var policy = await db.AgentPolicies.FirstOrDefaultAsync(x =>
                x.AgentId == dto.AgentId &&
                x.PolicyId == dto.PolicyId);

            if (policy == null)
                throw new KeyNotFoundException("Policy is not assigned to this agent");

            if (policy.Status != "Sold")
                throw new InvalidOperationException("Commission can only be calculated for a sold policy");

            var today = DateTime.UtcNow.Date;

            var rule = await db.CommissionRules
                .Where(x => x.ProductId == dto.ProductId &&
                            x.IsActive &&
                            x.EffectiveFrom.Date <= today &&
                            (x.EffectiveTo == null || x.EffectiveTo.Value.Date >= today))
                .OrderByDescending(x => x.EffectiveFrom)
                .FirstOrDefaultAsync();

            if (rule == null)
                throw new KeyNotFoundException("No active commission rule found for this product");

            var commission = new AgentCommission
            {
                AgentId = dto.AgentId,
                PolicyId = dto.PolicyId,
                PremiumAmount = dto.PremiumAmount,
                CommissionPercentage = rule.CommissionPercentage,
                CommissionAmount = dto.PremiumAmount * rule.CommissionPercentage / 100,
                Status = "Pending",
                EarnedDate = policy.SaleDate ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            await db.AgentCommissions.AddAsync(commission);
            await db.SaveChangesAsync();

            return mapper.Map<AgentCommissionDTO2>(commission);
        }

        public async Task<List<AgentCommissionDTO2>> FetchCommissions(
            string? status, long? agentId, DateTime? fromDate, DateTime? toDate)
        {
            var query = db.AgentCommissions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(status))
                query = query.Where(x => x.Status == status);

            if (agentId.HasValue)
                query = query.Where(x => x.AgentId == agentId.Value);

            if (fromDate.HasValue)
                query = query.Where(x => x.EarnedDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.EarnedDate <= toDate.Value);

            var data = await query.OrderByDescending(x => x.EarnedDate).ToListAsync();

            return mapper.Map<List<AgentCommissionDTO2>>(data);
        }

        public async Task<List<AgentCommissionDTO2>> FetchCommissionsByAgent(long agentId)
        {
            var agent = await db.Agents.FindAsync(agentId);

            if (agent == null)
                throw new KeyNotFoundException("Agent not found");

            var data = await db.AgentCommissions
                .Where(x => x.AgentId == agentId)
                .OrderByDescending(x => x.EarnedDate)
                .ToListAsync();

            return mapper.Map<List<AgentCommissionDTO2>>(data);
        }

        public async Task UpdateCommissionStatus(long commissionId, string status)
        {
            var commission = await db.AgentCommissions.FindAsync(commissionId);

            if (commission == null)
                throw new KeyNotFoundException("Commission not found");

            var allowedStatuses = new[] { "Pending", "Approved", "Reversed" };

            if (!allowedStatuses.Contains(status))
                throw new ArgumentException("Status must be Pending, Approved, or Reversed");

            if (commission.Status == "Reversed" && status != "Reversed")
                throw new InvalidOperationException("A reversed commission cannot be changed");

            commission.Status = status;

            await db.SaveChangesAsync();
        }
    }
}