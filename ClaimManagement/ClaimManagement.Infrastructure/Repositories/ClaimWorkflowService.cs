using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClaimManagement.Infrastructure.Repositories;

public class ClaimWorkflowService : IClaimWorkflowService
{
    ClaimDbContext db;
    IConfiguration configuration;
    ICurrentUser user;

    // key = claim type, value = amount above which a survey is required
    Dictionary<string, decimal> surveyThresholds = new();

    public ClaimWorkflowService(ClaimDbContext db, IConfiguration configuration, ICurrentUser user)
    {
        this.db = db;
        this.user = user;
        this.configuration = configuration;

        // BRD: "Motor claims require surveyor assignment above configurable claim amounts".
        // The BRD gives no amount, so the values come from appsettings.json (ClaimRules:SurveyThresholds).
        foreach (var item in configuration.GetSection("ClaimRules:SurveyThresholds").GetChildren())
        {
            if (decimal.TryParse(item.Value, out var amount))
                surveyThresholds[item.Key] = amount;
        }
    }

    public async Task ApproveClaimAsync(long id, ApproveClaimDTO dto, long performedBy)
    {
        // 1. Fetch the claim first so its properties are available for validation rules
        var claim = await GetClaimOrThrowAsync(id);

        // 2. Validate state machine transition constraints
        if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Approved))
            throw new ConflictException("Claim cannot be approved in its current status");

        // Recommendation: a decision waits until an open investigation is finished
        await CheckNoPendingInvestigationAsync(id, "approved");

        // 3. Fetch all documents associated with the claim
        var docs = await db.claim_documents.Where(d => d.claim_id == id).ToListAsync();

        // Recommendation: at least one document, and all of them Verified
        if (docs.Count == 0 || docs.Any(d => d.verification_status != "Verified"))
            throw new ConflictException("All claim documents must be uploaded and verified before approval");

        // BRD: mandatory documents vary by claim type. Each required document must be uploaded and Verified.
        var required = RequiredDocuments.For(configuration, claim.claim_type);
        var missing = required
            .Where(r => !docs.Any(d => d.verification_status == "Verified" &&
                                       d.document_name.Equals(r, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        if (missing.Count > 0)
            throw new ConflictException("Required documents are missing or not verified: " + string.Join(", ", missing));

        // BRD: "Surveyor report is mandatory before approval"
        var survey = await db.survey_assignment.FirstOrDefaultAsync(s => s.claim_id == id);
        if (survey != null && survey.survey_status != "Completed")
            throw new ConflictException("The survey report must be submitted before approval");

        // BRD: a survey is required above a configurable amount (Motor in the BRD).
        // Any other claim type with a threshold in appsettings.json is covered the same way.
        if (survey == null &&
            surveyThresholds.TryGetValue(claim.claim_type, out var limit) &&
            claim.estimated_loss > limit)
            throw new ConflictException("A surveyor must be assigned for this claim amount before approval");

        // Recommendation: the payable amount must exist before approval (Settlement needs it)
        bool hasAssessment = await db.claim_assessment.AnyAsync(a => a.claim_id == id);
        if (!hasAssessment)
            throw new ConflictException("Claim assessment must be completed before approval");

        // TODO: multi-level approval for high-value claims (BRD) is NOT done. It needs more approver roles.

        claim.claim_status = ClaimStatuses.Approved;
        db.claim_history.Add(CreateHistory(id, "Claim Approved", performedBy, dto.remarks));
        await db.SaveChangesAsync();
    }

    public async Task RejectClaimAsync(long id, RejectClaimDTO dto, long performedBy)
    {
        if (string.IsNullOrWhiteSpace(dto.reason))
            throw new BadRequestException("Reason is required to reject a claim");

        var claim = await GetClaimOrThrowAsync(id);

        if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Rejected))
            throw new ConflictException("Claim cannot be rejected in its current status");

        await CheckNoPendingInvestigationAsync(id, "rejected");

        // BRD: "Investigation report is mandatory before rejection due to fraud"
        if (dto.is_fraud)
        {
            bool reportSubmitted = await db.investigation.AnyAsync(i =>
                i.claim_id == id &&
                i.investigation_status == InvestigationStatuses.Completed &&
                i.report_path != null);

            if (!reportSubmitted)
                throw new ConflictException("An investigation report is required before rejecting a claim for fraud");
        }

        claim.claim_status = ClaimStatuses.Rejected;
        db.claim_history.Add(CreateHistory(id,
            dto.is_fraud ? "Claim Rejected (Fraud)" : "Claim Rejected", performedBy, dto.reason));
        await db.SaveChangesAsync();
    }

    public async Task CancelClaimAsync(long id, long performedBy)
    {
        var claim = await GetClaimOrThrowAsync(id);

        if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Cancelled))
            throw new ConflictException("Claim can be cancelled only before processing starts");

        claim.claim_status = ClaimStatuses.Cancelled;
        db.claim_history.Add(CreateHistory(id, "Claim Cancelled", performedBy, null));
        await db.SaveChangesAsync();
    }

    // ---------- helper methods ----------

    private async Task CheckNoPendingInvestigationAsync(long claimId, string decision)
    {
        bool pending = await db.investigation.AnyAsync(i =>
            i.claim_id == claimId && i.investigation_status == InvestigationStatuses.Pending);

        if (pending)
            throw new ConflictException("The investigation must be completed before the claim can be " + decision);
    }

    private async Task<ClaimMaster> GetClaimOrThrowAsync(long id)
    {
        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == id);
        if (claim == null)
            throw new NotFoundException("Claim not found");
        return claim;
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
