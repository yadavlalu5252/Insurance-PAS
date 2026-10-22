using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Helpers;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClaimManagement.Infrastructure.Repositories;

public class InvestigationService : IInvestigationService
{
    ClaimDbContext db;
    IMapper mapper;
    IFileStorageService storage;

    // recommendation (not in the BRD)
    static readonly string[] AllowedRecommendations = { "Genuine", "Fraud" };

    public InvestigationService(ClaimDbContext db, IMapper mapper, IFileStorageService storage)
    {
        this.db = db;
        this.mapper = mapper;
        this.storage = storage;
    }

    public async Task<InvestigationResponseDTO> CreateAsync(long claimId, CreateInvestigationDTO dto, long performedBy)
    {
        if (dto.investigator_id <= 0)
            throw new BadRequestException("Investigator id is required");
        if (string.IsNullOrWhiteSpace(dto.investigation_reason))
            throw new BadRequestException("Investigation reason is required");

        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
        if (claim == null)
            throw new NotFoundException("Claim not found");

        bool alreadyOpen = await db.investigation.AnyAsync(i =>
            i.claim_id == claimId && i.investigation_status == InvestigationStatuses.Pending);
        if (alreadyOpen)
            throw new ConflictException("An investigation is already open for this claim");

        if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.UnderInvestigation))
            throw new ConflictException("An investigation can be started only while the claim is Pending Verification or Under Survey");

        // TODO: investigator_id is an employee from another module. It is NOT validated yet.

        var inv = new InvestigationMaster
        {
            investigation_number = await GenerateNumberAsync(),
            claim_id = claimId,
            investigator_id = dto.investigator_id,
            investigation_reason = dto.investigation_reason,
            assigned_date = DateTime.Now,
            investigation_status = InvestigationStatuses.Pending
        };
        db.investigation.Add(inv);

        // the claim moves to Under Investigation, and both changes are saved together
        claim.claim_status = ClaimStatuses.UnderInvestigation;
        db.claim_history.Add(CreateHistory(claimId, "Investigation Started", performedBy,
            inv.investigation_number + ", investigator id: " + dto.investigator_id));

        await db.SaveChangesAsync();
        return mapper.Map<InvestigationResponseDTO>(inv);
    }

    public async Task<PagedResultDTO<InvestigationResponseDTO>> ListAsync(InvestigationFilterDTO filter)
    {
        if (filter.page_number < 1)
            throw new BadRequestException("Page number must be 1 or more");
        if (filter.page_size < 1 || filter.page_size > 100)
            throw new BadRequestException("Page size must be between 1 and 100");

        var query = db.investigation.AsQueryable();

        if (filter.claim_id.HasValue)
            query = query.Where(i => i.claim_id == filter.claim_id.Value);
        if (!string.IsNullOrWhiteSpace(filter.investigation_status))
            query = query.Where(i => i.investigation_status == filter.investigation_status);

        int total = await query.CountAsync();

        var data = await query
            .OrderByDescending(i => i.investigation_id)
            .Skip((filter.page_number - 1) * filter.page_size)
            .Take(filter.page_size)
            .ToListAsync();

        return new PagedResultDTO<InvestigationResponseDTO>
        {
            items = mapper.Map<List<InvestigationResponseDTO>>(data),
            page_number = filter.page_number,
            page_size = filter.page_size,
            total_records = total,
            total_pages = (int)Math.Ceiling(total / (double)filter.page_size)
        };
    }

    public async Task<InvestigationResponseDTO> GetByIdAsync(long id)
    {
        var inv = await GetOrThrowAsync(id);
        return mapper.Map<InvestigationResponseDTO>(inv);
    }

    public async Task<EvidenceResponseDTO> AddEvidenceAsync(long id, string? description,
        string? fileName, long fileLength, Stream? content, long performedBy)
    {
        if (string.IsNullOrWhiteSpace(description) || description.Length > 500)
            throw new BadRequestException("Description is required (max 500 characters)");

        string extension = FileValidator.Check(fileName, fileLength, content);

        var inv = await GetOrThrowAsync(id);
        if (inv.investigation_status != InvestigationStatuses.Pending)
            throw new ConflictException("Evidence cannot be added to a completed investigation");

        var evidence = new InvestigationEvidence
        {
            investigation_id = id,
            description = description,
            file_name = Path.GetFileName(fileName!),
            file_path = await storage.SaveFileAsync(content!, extension),
            uploaded_by = performedBy,
            uploaded_date = DateTime.Now
        };
        db.investigation_evidence.Add(evidence);
        db.claim_history.Add(CreateHistory(inv.claim_id, "Investigation Evidence Added", performedBy, description));

        await db.SaveChangesAsync();
        return mapper.Map<EvidenceResponseDTO>(evidence);
    }

    public async Task<List<EvidenceResponseDTO>> ListEvidenceAsync(long id)
    {
        await GetOrThrowAsync(id);

        var data = await db.investigation_evidence
            .Where(e => e.investigation_id == id)
            .OrderByDescending(e => e.uploaded_date)
            .ToListAsync();

        return mapper.Map<List<EvidenceResponseDTO>>(data);
    }

    public async Task<InvestigationResponseDTO> SubmitReportAsync(long id, string? findings, string? recommendation,
        string? fileName, long fileLength, Stream? content, long performedBy)
    {
        if (string.IsNullOrWhiteSpace(findings))
            throw new BadRequestException("Findings are required");
        if (string.IsNullOrWhiteSpace(recommendation) || !AllowedRecommendations.Contains(recommendation))
            throw new BadRequestException("Recommendation must be Genuine or Fraud");

        string extension = FileValidator.Check(fileName, fileLength, content);

        var inv = await GetOrThrowAsync(id);
        if (inv.investigation_status != InvestigationStatuses.Pending)
            throw new ConflictException("The report of this investigation is already submitted");

        inv.report_path = await storage.SaveFileAsync(content!, extension);
        inv.findings = findings;
        inv.recommendation = recommendation;
        inv.investigation_status = InvestigationStatuses.Completed;
        inv.completed_date = DateTime.Now;

        // the claim stays Under Investigation. Admin now approves or rejects it.
        db.claim_history.Add(CreateHistory(inv.claim_id, "Investigation Report Submitted", performedBy,
            inv.investigation_number + ", recommendation: " + recommendation));

        await db.SaveChangesAsync();
        return mapper.Map<InvestigationResponseDTO>(inv);
    }

    // ---------- helper methods ----------

    private async Task<InvestigationMaster> GetOrThrowAsync(long id)
    {
        var inv = await db.investigation.FirstOrDefaultAsync(i => i.investigation_id == id);
        if (inv == null)
            throw new NotFoundException("Investigation not found");
        return inv;
    }

    private async Task<string> GenerateNumberAsync()
    {
        string number;
        do
        {
            number = "INV-" + DateTime.Now.ToString("yyyyMMdd") + "-" + Random.Shared.Next(1000, 10000);
        }
        while (await db.investigation.AnyAsync(i => i.investigation_number == number));

        return number;
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