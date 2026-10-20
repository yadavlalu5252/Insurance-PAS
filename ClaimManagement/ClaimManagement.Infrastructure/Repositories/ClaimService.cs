using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Infrastructure.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class ClaimService : IClaimService
    {
        ClaimDbContext db;
        IMapper mapper;

        public ClaimService(ClaimDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task<ClaimResponseDto> AddClaimAsync(ClaimCreateDto dto, long performedBy)
        {
            // TODO: policy and premium eligibility check goes here.
            // It is NOT done yet, so claims are saved WITHOUT policy validation.

            var claim = mapper.Map<ClaimMaster>(dto);
            claim.claim_number = await GenerateClaimNumberAsync();
            claim.claim_date = DateTime.Now;
            claim.claim_status = ClaimStatuses.Registered;

            // the claim and its first history row are saved together
            claim.claim_history.Add(CreateHistory("Claim Registered", performedBy));

            db.claim_master.Add(claim);
            await db.SaveChangesAsync();

            return mapper.Map<ClaimResponseDto>(claim);
        }

        public async Task<List<ClaimResponseDto>> FetchClaimsAsync()
        {
            var data = await db.claim_master
                .OrderByDescending(c => c.claim_id)
                .ToListAsync();
            return mapper.Map<List<ClaimResponseDto>>(data);
        }

        public async Task<ClaimResponseDto?> FetchClaimByIdAsync(long id)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
            return claim == null ? null : mapper.Map<ClaimResponseDto>(claim);
        }

        public async Task<ClaimStatusResponseDto?> FetchClaimStatusAsync(long id)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
            return claim == null ? null : mapper.Map<ClaimStatusResponseDto>(claim);
        }

        public async Task<string> UpdateClaimAsync(long id, ClaimUpdateDto dto, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
            if (claim == null) return "NotFound";

            // Recommendation (not in BRD): edit only before processing starts
            if (claim.claim_status != ClaimStatuses.Registered &&
                claim.claim_status != ClaimStatuses.PendingVerification)
                return "NotAllowed";

            mapper.Map(dto, claim);

            db.claim_history.Add(CreateHistory("Claim Updated", performedBy, claim.claim_id));
            await db.SaveChangesAsync();
            return "Success";
        }

        // ---------- helper methods ----------

        private async Task<string> GenerateClaimNumberAsync()
        {
            string number;
            do
            {
                number = "CLM-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Random.Shared.Next(1000, 10000);
            }
            while (await db.claim_master.AnyAsync(c => c.claim_number == number));

            return number;
        }

        private static ClaimHistory CreateHistory(string action, long performedBy, long claimId = 0)
        {
            return new ClaimHistory
            {
                claim_id = claimId,
                action = action,
                performed_by = performedBy,
                action_date = DateTime.Now
            };
        }
    }
}
