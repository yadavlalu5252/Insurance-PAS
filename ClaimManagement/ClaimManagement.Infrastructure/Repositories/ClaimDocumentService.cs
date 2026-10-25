using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Helpers;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Infrastructure.Config;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ClaimManagement.Infrastructure.Repositories;

public class ClaimDocumentService : IClaimDocumentService
{
    ClaimDbContext db;
    IMapper mapper;
    IFileStorageService storage;
    IConfiguration configuration;

    public ClaimDocumentService(ClaimDbContext db, IMapper mapper, IFileStorageService storage, IConfiguration configuration)
    {
        this.db = db;
        this.mapper = mapper;
        this.storage = storage;
        this.configuration = configuration;
    }

    public async Task<DocumentResponseDTO> AddDocumentAsync(long claimId, string? documentName,
        string? fileName, long fileLength, Stream? content, long performedBy)
    {
        if (string.IsNullOrWhiteSpace(documentName) || documentName.Length > 150)
            throw new BadRequestException("Document name is required (max 150 characters)");

        string extension = FileValidator.Check(fileName, fileLength, content);

        var claim = await GetClaimOrThrowAsync(claimId);

        // Recommendation: no uploads after a final decision
        if (claim.claim_status == ClaimStatuses.Approved ||
            claim.claim_status == ClaimStatuses.Rejected ||
            claim.claim_status == ClaimStatuses.Settled ||
            claim.claim_status == ClaimStatuses.Cancelled)
            throw new ConflictException("Documents cannot be added in the current claim status");

        string storedName = await storage.SaveFileAsync(content!, extension);

        var doc = new ClaimDocument
        {
            claim_id = claimId,
            document_name = documentName,
            file_path = storedName,
            verification_status = "Pending",
            uploaded_date = DateTime.Now
        };
        db.claim_documents.Add(doc);

        string? remarks = null;

        // Recommendation (not in the BRD): the first upload moves the claim to Pending Verification
        if (claim.claim_status == ClaimStatuses.Registered)
        {
            claim.claim_status = ClaimStatuses.PendingVerification;
            remarks = "Status changed to Pending Verification";
        }

        db.claim_history.Add(CreateHistory(claimId, "Document Uploaded: " + documentName, performedBy, remarks));
        await db.SaveChangesAsync();

        return mapper.Map<DocumentResponseDTO>(doc);
    }

    public async Task<List<string>> FetchRequiredAsync(long claimId)
    {
        var claim = await GetClaimOrThrowAsync(claimId);
        return RequiredDocuments.For(configuration, claim.claim_type);
    }

    public async Task<List<DocumentResponseDTO>> FetchDocumentsAsync(long claimId)
    {
        await GetClaimOrThrowAsync(claimId);

        var docs = await db.claim_documents
            .Where(d => d.claim_id == claimId)
            .OrderByDescending(d => d.uploaded_date)
            .ToListAsync();

        return mapper.Map<List<DocumentResponseDTO>>(docs);
    }

    public async Task VerifyDocumentAsync(long claimId, VerifyDocumentDTO dto, long performedBy)
    {
        if (dto.document_id <= 0)
            throw new BadRequestException("Document id is required");
        if (dto.verification_status != "Verified" && dto.verification_status != "Rejected")
            throw new BadRequestException("Verification status must be Verified or Rejected");

        await GetClaimOrThrowAsync(claimId);

        var doc = await db.claim_documents
            .FirstOrDefaultAsync(d => d.document_id == dto.document_id && d.claim_id == claimId);
        if (doc == null)
            throw new NotFoundException("Document not found for this claim");

        doc.verification_status = dto.verification_status;

        db.claim_history.Add(CreateHistory(claimId,
            "Document " + dto.verification_status + ": " + doc.document_name,
            performedBy, dto.remarks));

        await db.SaveChangesAsync();
    }

    private async Task<ClaimMaster> GetClaimOrThrowAsync(long claimId)
    {
        var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
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