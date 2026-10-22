using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Infrastructure.Repositories;

public class AssessmentService : IAssessmentService
{
    ClaimDbContext db;
    IMapper mapper;

    public AssessmentService(ClaimDbContext db, IMapper mapper)
    {
        this.db = db;
        this.mapper = mapper;
    }

    public async Task<AssessmentResponseDTO> SaveAssessmentAsync(long claimId, CreateAssessmentDTO dto, long performedBy)
    {
        if (dto.assessed_amount <= 0)
            throw new BadRequestException("Assessed amount must be greater than 0");
        if (dto.deductible < 0 || dto.depreciation < 0 || dto.non_payable < 0)
            throw new BadRequestException("Deductible, depreciation and non-payable cannot be negative");

        // BRD formula: Net Payable = Claim Amount - Deductible - Depreciation - Non Payable
        decimal net = dto.assessed_amount - dto.deductible - dto.depreciation - dto.non_payable;
        if (net <= 0)
            throw new BadRequestException("Net payable amount must be greater than 0");

        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
        if (claim == null)
            throw new NotFoundException("Claim not found");

        // assessment is allowed only while the claim is still being processed
        if (claim.claim_status != ClaimStatuses.PendingVerification &&
            claim.claim_status != ClaimStatuses.UnderSurvey &&
            claim.claim_status != ClaimStatuses.UnderInvestigation)
            throw new ConflictException("Assessment is allowed only while the claim is being processed");

        // TODO: "settlement cannot exceed sum insured" needs the Policy service. Checked at settlement time.

        var existing = await db.claim_assessment.FirstOrDefaultAsync(a => a.claim_id == claimId);

        if (existing == null)
        {
            existing = new ClaimAssessment { claim_id = claimId };
            db.claim_assessment.Add(existing);
        }

        existing.assessed_amount = dto.assessed_amount;
        existing.deductible = dto.deductible;
        existing.depreciation = dto.depreciation;
        existing.non_payable = dto.non_payable;
        existing.net_payable = net;
        existing.assessed_by = performedBy;
        existing.assessed_date = DateTime.Now;

        db.claim_history.Add(new ClaimHistory
        {
            claim_id = claimId,
            action = "Claim Assessed",
            performed_by = performedBy,
            action_date = DateTime.Now,
            remarks = "Net payable amount: " + net
        });

        await db.SaveChangesAsync();
        return mapper.Map<AssessmentResponseDTO>(existing);
    }

    public async Task<AssessmentResponseDTO> FetchAssessmentAsync(long claimId)
    {
        var a = await db.claim_assessment.FirstOrDefaultAsync(x => x.claim_id == claimId);
        if (a == null)
            throw new NotFoundException("No assessment found for this claim");

        return mapper.Map<AssessmentResponseDTO>(a);
    }
}