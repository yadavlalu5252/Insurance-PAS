
using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Domain.Entities;
using PolicyIssuance.Domain.Enums;
using PolicyIssuance.Infrastructure.Data;

namespace PolicyIssuance.Infrastructure.Services
{
    public class PolicyEndorsementService : IPolicyEndorsementService
    {
        private readonly AppDbContext db;

        public PolicyEndorsementService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<PolicyEndorsementDTO?> CreateAsync(
            long policy_id, PolicyEndorsementDTO dto)
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.policy_id == policy_id);

            if (policy == null ||
                policy.policy_status != PolicyStatus.Active ||
                string.IsNullOrWhiteSpace(dto.endorsement_type) ||
                string.IsNullOrWhiteSpace(dto.new_values))
            {
                return null;
            }

            var endorsement = new PolicyEndorsement
            {
                policy_id = policy_id,
                endorsement_number = "END-" +
                    Guid.NewGuid().ToString("N").ToUpperInvariant(),
                endorsement_type = dto.endorsement_type,
                old_values = dto.old_values ?? string.Empty,
                new_values = dto.new_values,
                status = "Pending",
                approved_by = null,
                created_date = DateTime.UtcNow,
                approved_date = null
            };

            db.PolicyEndorsements.Add(endorsement);
            await db.SaveChangesAsync();

            return ToDTO(endorsement);
        }

        public async Task<List<PolicyEndorsementDTO>> GetByPolicyIdAsync(
            long policy_id)
        {
            var endorsements = await db.PolicyEndorsements
                .AsNoTracking()
                .Where(e => e.policy_id == policy_id)
                .OrderByDescending(e => e.created_date)
                .ToListAsync();

            return endorsements.Select(ToDTO).ToList();
        }

        public async Task<bool> ApproveAsync(
            long endorsement_id, string approved_by)
        {
            if (string.IsNullOrWhiteSpace(approved_by))
                return false;

            var endorsement = await db.PolicyEndorsements
                .FirstOrDefaultAsync(e => e.endorsement_id == endorsement_id);

            if (endorsement == null || endorsement.status != "Pending")
                return false;

            endorsement.status = "Approved";
            endorsement.approved_by = approved_by;
            endorsement.approved_date = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectAsync(
            long endorsement_id, string approved_by)
        {
            if (string.IsNullOrWhiteSpace(approved_by))
                return false;

            var endorsement = await db.PolicyEndorsements
                .FirstOrDefaultAsync(e => e.endorsement_id == endorsement_id);

            if (endorsement == null || endorsement.status != "Pending")
                return false;

            endorsement.status = "Rejected";
            endorsement.approved_by = approved_by;
            endorsement.approved_date = DateTime.UtcNow;

            await db.SaveChangesAsync();
            return true;
        }

        private static PolicyEndorsementDTO ToDTO(
            PolicyEndorsement endorsement)
        {
            return new PolicyEndorsementDTO
            {
                endorsement_id = endorsement.endorsement_id,
                policy_id = endorsement.policy_id,
                endorsement_number = endorsement.endorsement_number,
                endorsement_type = endorsement.endorsement_type,
                old_values = endorsement.old_values,
                new_values = endorsement.new_values,
                status = endorsement.status,
                approved_by = endorsement.approved_by,
                created_date = endorsement.created_date,
                approved_date = endorsement.approved_date
            };
        }
    }
}
