

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Domain.Entities;
using PolicyIssuance.Infrastructure.Data;

namespace PolicyIssuance.Infrastructure.Services
{ 
    public class PolicyClosureService : IPolicyClosureService
    {
        AppDbContext db;
        IMapper mapper;

        public PolicyClosureService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddClosure(PolicyClosureDTO dto)
        {
            if (dto.policy_id <= 0)
                throw new ArgumentException("Invalid policy ID.");

            if (string.IsNullOrWhiteSpace(dto.closure_reason))
                throw new ArgumentException("Closure reason is required.");

            if (string.IsNullOrWhiteSpace(dto.requested_by))
                throw new ArgumentException("Requester is required.");

            var pendingExists = await db.policyclosures.AnyAsync(
                x => x.policy_id == dto.policy_id &&
                     x.closure_status == "Pending");

            if (pendingExists)
                throw new ArgumentException(
                    "A pending closure request already exists for this policy.");

            var data = mapper.Map<PolicyClosure>(dto);

            data.policy_closure_id = 0;
            data.closure_reason = dto.closure_reason.Trim();
            data.requested_by = dto.requested_by.Trim();
            data.requested_at = DateTime.UtcNow;
            data.closure_status = "Pending";

            await db.policyclosures.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<PolicyClosureDTO2?> FindClosureById(long id)
        {
            var data = await db.policyclosures.FindAsync(id);

            if (data == null)
                return null;

            return mapper.Map<PolicyClosureDTO2>(data);
        }

        public async Task<List<PolicyClosureDTO2>> FetchAllClosure()
        {
            var data = await db.policyclosures
                .OrderByDescending(x => x.requested_at)
                .ToListAsync();

            return mapper.Map<List<PolicyClosureDTO2>>(data);
        }

        public async Task<List<PolicyClosureDTO2>> FetchPendingClosure()
        {
            var data = await db.policyclosures
                .Where(x => x.closure_status == "Pending")
                .OrderByDescending(x => x.requested_at)
                .ToListAsync();

            return mapper.Map<List<PolicyClosureDTO2>>(data);
        }

        public async Task<bool> ApproveClosure(
            long id, string approvedBy, decimal? refundAmount)
        {
            if (string.IsNullOrWhiteSpace(approvedBy))
                return false;

            if (refundAmount.HasValue && refundAmount.Value < 0)
                return false;

            var data = await db.policyclosures.FindAsync(id);

            if (data == null || data.closure_status != "Pending")
                return false;

            data.approved_by = approvedBy.Trim();
            data.approved_at = DateTime.UtcNow;
            data.refund_amount = refundAmount;
            data.closed_at = DateTime.UtcNow;
            data.closure_status = "Closed";

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> RejectClosure(
            long id, string rejectionReason, string rejectedBy)
        {
            if (string.IsNullOrWhiteSpace(rejectionReason) ||
                string.IsNullOrWhiteSpace(rejectedBy))
                return false;

            var data = await db.policyclosures.FindAsync(id);

            if (data == null || data.closure_status != "Pending")
                return false;

            data.rejection_reason = rejectionReason.Trim();
            data.approved_by = rejectedBy.Trim();
            data.approved_at = DateTime.UtcNow;
            data.closure_status = "Rejected";

            await db.SaveChangesAsync();

            return true;
        }
    }
}
