using AutoMapper;
using Microsoft.Data.SqlClient;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class ClaimService : IClaimService
    {
        ClaimDbContext db;
        IMapper mapper;
        INotificationService notification;
        PolicyClient policyClient;
        PremiumClient premiumClient;
        ILogger<ClaimService> logger;

        public ClaimService(ClaimDbContext db, IMapper mapper, INotificationService notification,
            PolicyClient policyClient, PremiumClient premiumClient, ILogger<ClaimService> logger)
        {
            this.db = db;
            this.mapper = mapper;
            this.notification = notification;
            this.policyClient = policyClient;
            this.premiumClient = premiumClient;
            this.logger = logger;
        }

        public async Task<(string Result, ClaimResponseDto? Claim, string? Reason)> AddClaimAsync(ClaimCreateDto dto, long performedBy)
        {
            string? reason;

            try
            {
                reason = await CheckEligibilityAsync(dto);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is TaskCanceledException || ex is System.Text.Json.JsonException)
            {
                logger.LogError(ex, "Policy or premium service call failed for policy {PolicyId}", dto.policy_id);
                return ("ServiceUnavailable", null, "Policy or premium service is not available right now");
            }

            if (reason != null)
                return ("NotEligible", null, reason);

            for (int attempt = 1; attempt <= 3; attempt++)
            {
                var claim = mapper.Map<ClaimMaster>(dto);
                claim.claim_number = await GenerateClaimNumberAsync();
                claim.claim_date = DateTime.Now;
                claim.claim_status = ClaimStatuses.Registered;
                claim.claim_history.Add(CreateHistory("Claim Registered", performedBy));

                db.claim_master.Add(claim);

                try
                {
                    await db.SaveChangesAsync();

                    await notification.NotifyAsync(claim.customer_id, NotificationEvents.ClaimRegistered,
                        "Your claim " + claim.claim_number + " has been registered.");

                    return ("Success", mapper.Map<ClaimResponseDto>(claim), null);
                }
                catch (DbUpdateException ex) when (IsDuplicateKey(ex) && attempt < 3)
                {
                    db.ChangeTracker.Clear();
                }
            }

            throw new InvalidOperationException("Could not generate a unique claim number");
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

        public async Task<List<ClaimHistoryDTO>?> FetchClaimHistoryAsync(long id)
        {
            bool exists = await db.claim_master.AnyAsync(c => c.claim_id == id);
            if (!exists) return null;

            var data = await db.claim_history
                .Where(h => h.claim_id == id)
                .OrderByDescending(h => h.action_date)
                .ThenByDescending(h => h.history_id)
                .ToListAsync();

            return mapper.Map<List<ClaimHistoryDTO>>(data);
        }

        //helper methods

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

        // returns null if eligible, otherwise the reason
        private async Task<string?> CheckEligibilityAsync(ClaimCreateDto dto)
        {
            // policy checks (Anuja)
            var policy = await policyClient.FindPolicyById(dto.policy_id);
            if (policy == null)
                return "Policy not found";

            if (policy.customer_id != dto.customer_id)
                return "Policy does not belong to this customer";

            if (policy.policy_status == "Expired")
                return "Policy has expired";

            if (policy.policy_status != "Active")
                return "Policy is not active";

            var incident = dto.incident_date.Date;
            if (incident < policy.effective_date.Date || incident > policy.expiry_date.Date)
                return "Incident date is outside the policy period";

            // premium check (Khushi)
            var schedule = await premiumClient.FindScheduleByPolicy(dto.policy_id);
            if (schedule == null || schedule.Count == 0)
                return "No premium schedule found for this policy";

            if (schedule.Any(s => s.status != "Paid" && s.status != "Cancelled" && s.due_date.Date < incident))
                return "Premium is not paid";
                return null;    
        }

        private static bool IsDuplicateKey(DbUpdateException ex)
        {
            return ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
        }
    }
}
