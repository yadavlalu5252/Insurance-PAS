using AutoMapper;
using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class ClaimDocumentService : IClaimDocumentService
    {
        ClaimDbContext db;
        IMapper mapper;
        IFileStorageService storage;
        INotificationService notification;

        public ClaimDocumentService(ClaimDbContext db, IMapper mapper, IFileStorageService storage, INotificationService notification)
        {
            this.db = db;
            this.mapper = mapper;
            this.storage = storage;
            this.notification = notification;
        }

        public async Task<(string result, DocumentResponseDTO? data)> AddDocumentAsync(
            long claimId, string documentName, string extension, Stream content, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
            if (claim == null) return ("NotFound", null);

            // Recommendation: no uploads after a final decision
            if (claim.claim_status == ClaimStatuses.Approved ||
                claim.claim_status == ClaimStatuses.Rejected ||
                claim.claim_status == ClaimStatuses.Settled ||
                claim.claim_status == ClaimStatuses.Cancelled)
                return ("NotAllowed", null);

            string storedName = await storage.SaveFileAsync(content, extension);

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

            // Recommendation (not stated in the BRD): first upload moves the claim to Pending Verification
            if (claim.claim_status == ClaimStatuses.Registered)
            {
                claim.claim_status = ClaimStatuses.PendingVerification;
                remarks = "Status changed to Pending Verification";
            }

            db.claim_history.Add(CreateHistory(claimId, "Document Uploaded: " + documentName, performedBy, remarks));

            await db.SaveChangesAsync();

            return ("Success", mapper.Map<DocumentResponseDTO>(doc));
        }

        public async Task<List<DocumentResponseDTO>?> FetchDocumentsAsync(long claimId)
        {
            bool exists = await db.claim_master.AnyAsync(c => c.claim_id == claimId);
            if (!exists) return null;

            var docs = await db.claim_documents
                .Where(d => d.claim_id == claimId)
                .OrderByDescending(d => d.uploaded_date)
                .ToListAsync();

            return mapper.Map<List<DocumentResponseDTO>>(docs);
        }

        public async Task<string> VerifyDocumentAsync(long claimId, VerifyDocumentDTO dto, long performedBy)
        {
            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
            if (claim == null) return "ClaimNotFound";

            var doc = await db.claim_documents
                .FirstOrDefaultAsync(d => d.document_id == dto.document_id && d.claim_id == claimId);
            if (doc == null) return "DocumentNotFound";

            doc.verification_status = dto.verification_status;

            db.claim_history.Add(CreateHistory(claimId,
                "Document " + dto.verification_status + ": " + doc.document_name,
                performedBy, dto.remarks));

            await db.SaveChangesAsync();

            // a rejected document means the customer must upload it again
            if (dto.verification_status == "Rejected")
            {
                await notification.NotifyAsync(claim.customer_id, NotificationEvents.DocumentsPending,
                    "Document '" + doc.document_name + "' was rejected. Please upload it again.");
            }

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