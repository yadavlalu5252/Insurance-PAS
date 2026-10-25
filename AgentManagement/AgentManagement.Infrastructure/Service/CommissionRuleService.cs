
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AgentManagement.Application.DTO;
using AgentManagement.Application.Interface;
using AgentManagement.Domain.Entities;
using AgentManagement.Infrastructure.Data;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AgentManagement.Infrastructure.Service
{
    public class CommissionRuleService : ICommissionRuleService
    {
        AppDbContext db;
        IMapper mapper;

        public CommissionRuleService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddRule(CommissionRuleDTO dto)
        {
            ValidateRule(dto);

            var data = mapper.Map<CommissionRule>(dto);

            data.CreatedAt = DateTime.UtcNow;

            await db.CommissionRules.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<List<CommissionRuleDTO2>> FetchRules()
        {
            // Return all rules, including inactive and historical rules.
            var data = await db.CommissionRules
                .AsNoTracking()
                .OrderBy(x => x.ProductId)
                .ThenByDescending(x => x.EffectiveFrom)
                .ToListAsync();

            return mapper.Map<List<CommissionRuleDTO2>>(data);
        }

        public async Task<List<CommissionRuleDTO2>> FetchRulesByProduct(
            long productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID.");

            var today = DateTime.UtcNow.Date;

            // Return only rules that are active and effective today.
            var data = await db.CommissionRules
                .AsNoTracking()
                .Where(x =>
                    x.ProductId == productId &&
                    x.IsActive &&
                    x.EffectiveFrom.Date <= today &&
                    (x.EffectiveTo == null ||
                     x.EffectiveTo.Value.Date >= today))
                .OrderByDescending(x => x.EffectiveFrom)
                .ToListAsync();

            return mapper.Map<List<CommissionRuleDTO2>>(data);
        }

        public async Task UpdateRule(long ruleId, CommissionRuleDTO dto)
        {
            if (ruleId <= 0)
                throw new ArgumentException("Invalid commission rule ID.");

            ValidateRule(dto);

            var data = await db.CommissionRules.FindAsync(ruleId);

            if (data == null)
                throw new KeyNotFoundException(
                    "Commission rule not found.");

            // Update editable fields without changing ID or CreatedAt.
            mapper.Map(dto, data);

            await db.SaveChangesAsync();
        }

        private static void ValidateRule(CommissionRuleDTO dto)
        {
            if (dto.ProductId <= 0)
                throw new ArgumentException("Invalid product ID.");

            if (dto.CommissionPercentage < 0 ||
                dto.CommissionPercentage > 100)
            {
                throw new ArgumentException(
                    "Commission percentage must be between 0 and 100.");
            }

            if (dto.EffectiveTo.HasValue &&
                dto.EffectiveTo.Value.Date < dto.EffectiveFrom.Date)
            {
                throw new ArgumentException(
                    "EffectiveTo cannot be earlier than EffectiveFrom.");
            }
        }
    }
}
