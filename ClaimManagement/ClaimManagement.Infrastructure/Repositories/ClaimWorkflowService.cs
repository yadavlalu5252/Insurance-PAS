using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class ClaimWorkflowService : IClaimWorkflowService
    {
        ClaimDbContext db;
        INotificationService notification;

        public ClaimWorkflowService(ClaimDbContext db, INotificationService notification)
        {
            this.db = db;
            this.notification = notification;
        }

        public async Task<string> ApproveClaimAsync(long id, ApproveClaimDTO dto, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
            if (claim == null) return "NotFound";

            if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Approved))
                return "InvalidStatus";

            // at least one document, and all of them Verified
            var docs = await db.claim_documents.Where(d => d.claim_id == id).ToListAsync();
            if (docs.Count == 0 || docs.Any(d => d.verification_status != "Verified"))
                return "DocumentsNotVerified";

            // BRD: surveyor report mandatory before approval (only for claims that have a survey)
            var survey = await db.survey_assignment.FirstOrDefaultAsync(s => s.claim_id == id);
            if (survey != null && survey.survey_status != SurveyStatuses.Completed)
                return "SurveyPending";

            // TODO: multi-level approval for high-value claims is NOT done (needs more roles).

            claim.claim_status = ClaimStatuses.Approved;
            db.claim_history.Add(CreateHistory(id, "Claim Approved", performedBy, dto.remarks));
            await db.SaveChangesAsync();

            await notification.NotifyAsync(claim.customer_id, NotificationEvents.ClaimApproved,
                "Your claim " + claim.claim_number + " has been approved.");

            return "Success";
        }

        public async Task<string> RejectClaimAsync(long id, RejectClaimDTO dto, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
            if (claim == null) return "NotFound";

            if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Rejected))
                return "InvalidStatus";

            // TODO: "investigation report mandatory before fraud rejection" (BRD) is NOT done.
            // Investigation belongs to another teammate.

            claim.claim_status = ClaimStatuses.Rejected;
            db.claim_history.Add(CreateHistory(id, "Claim Rejected", performedBy, dto.reason));
            await db.SaveChangesAsync();

            await notification.NotifyAsync(claim.customer_id, NotificationEvents.ClaimRejected,
                "Your claim " + claim.claim_number + " has been rejected. Reason: " + dto.reason);

            return "Success";
        }

        public async Task<string> CancelClaimAsync(long id, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
            if (claim == null) return "NotFound";

            if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Cancelled))
                return "InvalidStatus";

            claim.claim_status = ClaimStatuses.Cancelled;
            db.claim_history.Add(CreateHistory(id, "Claim Cancelled", performedBy, null));
            await db.SaveChangesAsync();
            return "Success";
        }

        private static ClaimHistory CreateHistory(long claimId, string action, long performedBy, string? remarks)
        {
            return new ClaimHistory
            {
                claim_id = claimId,
                action = action,
                performed_by = performedBy,
                action_date = DateTime.Now,
                remarks = remarks
            };
        }
    }
}