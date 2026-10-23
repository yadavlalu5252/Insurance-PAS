using AutoMapper;
using KycVerification.Application.DTO;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using KycVerification.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace KycVerification.Infrastructure.Repositories
{
    public class KycService : IKycService
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;

        public KycService(AppDbContext db, IMapper mapper, IWebHostEnvironment env)
        {
            this.db = db;
            this.mapper = mapper;
            this.env = env;
        }

        public async Task<CustomerKycDto> CreateKycAsync(CreateCustomerKycDto dto)
        {
            var existing = await db.CustomerKycs
                .AnyAsync(x => x.customer_id == dto.CustomerId &&
                    (x.verification_status == "Pending" ||
                     x.verification_status == "Re-Submitted"));

            if (existing)
                throw new InvalidOperationException(
                    "This customer already has an active KYC request.");

            var kyc = mapper.Map<CustomerKyc>(dto);

            kyc.kyc_id = 0;
            kyc.kyc_number = $"KYC-{Guid.NewGuid():N}";
            kyc.kyc_date = DateTime.UtcNow;
            kyc.verification_status = "Pending";
            kyc.verified_by = null;
            kyc.verified_date = null;

            db.CustomerKycs.Add(kyc);
            await db.SaveChangesAsync();

            await AddLogAsync(kyc.kyc_id, "KYC Created",
                null, "KYC request created.");

            return mapper.Map<CustomerKycDto>(kyc);
        }

        public async Task<CustomerKycDto?> GetKycByIdAsync(int kycId)
        {
            var kyc = await db.CustomerKycs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.kyc_id == kycId);

            return kyc == null
                ? null
                : mapper.Map<CustomerKycDto>(kyc);
        }

        public async Task<List<CustomerKycDto>> GetAllKycAsync()
        {
            var records = await db.CustomerKycs
                .AsNoTracking()
                .OrderByDescending(x => x.kyc_date)
                .ToListAsync();

            return mapper.Map<List<CustomerKycDto>>(records);
        }

        public async Task<KycDocumentDto> UploadDocumentAsync(UploadKycDocumentDto dto)
        {
            var kyc = await db.CustomerKycs
                .FirstOrDefaultAsync(x => x.kyc_id == dto.KycId);

            if (kyc == null)
                throw new KeyNotFoundException("KYC record not found.");

            if (kyc.verification_status == "Verified")
                throw new InvalidOperationException(
                    "Documents cannot be uploaded to an already verified KYC.");

            var file = dto.File;

            if (file == null || file.Length == 0)
                throw new ArgumentException("Please select a document.");

            const long maxFileSize = 5L * 1024 * 1024;

            if (file.Length > maxFileSize)
                throw new ArgumentException("Maximum file size allowed is 5 MB.");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException(
                    "Only PDF, JPG, JPEG and PNG files are allowed.");

            var folder = Path.Combine(env.ContentRootPath, "SecureDocuments");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folder, fileName);

            try
            {
                await using (var stream = new FileStream(
                    fullPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new KycDocument
                {
                    KycId = (int)dto.KycId,
                    DocumentType = dto.DocumentType,
                    DocumentNumber = dto.DocumentNumber,
                    FilePath = Path.Combine("SecureDocuments", fileName),
                    UploadDate = DateTime.UtcNow,
                    VerificationStatus = "Pending"
                };

                db.KycDocuments.Add(document);
                await db.SaveChangesAsync();

                await AddLogAsync((int)dto.KycId, "Document Uploaded",
                    null, $"Document uploaded: {dto.DocumentType}");

                return mapper.Map<KycDocumentDto>(document);
            }
            catch
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                throw;
            }
        }

        public async Task<bool> ApproveKycAsync(int kycId, ReviewKycDto dto)
        {
            var kyc = await db.CustomerKycs
                .FirstOrDefaultAsync(x => x.kyc_id == kycId);

            if (kyc == null)
                return false;

            var documents = await db.KycDocuments
                .Where(x => x.KycId == kycId)
                .ToListAsync();

            var mandatoryTypes = await db.DocumentTypeMasters
                .Where(x => x.MandatoryFlag && x.Status == "Active")
                .Select(x => x.DocumentName)
                .ToListAsync();

            foreach (var documentType in mandatoryTypes)
            {
                var document = documents.FirstOrDefault(
                    x => x.DocumentType == documentType &&
                         x.VerificationStatus == "Verified");

                if (document == null)
                    throw new InvalidOperationException(
                        $"{documentType} is mandatory and must be uploaded and verified.");
            }

            if (documents.Any(x => x.VerificationStatus != "Verified"))
                throw new InvalidOperationException(
                    "All uploaded documents must be verified before KYC approval.");

            kyc.verification_status = "Verified";
            kyc.verified_date = DateTime.UtcNow;
            kyc.remarks = dto.Remarks;

            await db.SaveChangesAsync();

            await AddLogAsync(kycId, "KYC Approved",
                null, dto.Remarks ?? "KYC approved successfully.");

            return true;
        }

        public async Task<bool> RejectKycAsync(int kycId, ReviewKycDto dto)
        {
            var kyc = await db.CustomerKycs
                .FirstOrDefaultAsync(x => x.kyc_id == kycId);

            if (kyc == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.Remarks))
                throw new ArgumentException("Remarks are required when rejecting KYC.");

            kyc.verification_status = "Rejected";
            kyc.remarks = dto.Remarks;
            kyc.verified_date = DateTime.UtcNow;

            await db.SaveChangesAsync();

            await AddLogAsync(kycId, "KYC Rejected", null, dto.Remarks);

            return true;
        }

        public async Task<List<KycDocumentDto>> GetDocumentsAsync(int kycId)
        {
            var documents = await db.KycDocuments
                .AsNoTracking()
                .Where(x => x.KycId == kycId)
                .OrderByDescending(x => x.UploadDate)
                .ToListAsync();

            return mapper.Map<List<KycDocumentDto>>(documents);
        }

        // ✅ Uses snake_case for KycVerificationLog (matches entity)
        private async Task AddLogAsync(
            int kycId, string action, int? performedBy, string? remarks)
        {
            var log = new KycVerificationLog
            {
                kyc_id = kycId,
                action = action,
                performed_by = performedBy,
                action_date = DateTime.UtcNow,
                remarks = remarks
            };

            db.KycVerificationLogs.Add(log);
            await db.SaveChangesAsync();
        }

        public async Task<bool> VerifyDocumentAsync(int documentId, ReviewKycDto dto)
        {
            var document = await db.KycDocuments
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);

            if (document == null)
                return false;

            document.VerificationStatus = "Verified";

            await db.SaveChangesAsync();

            await AddLogAsync(
                document.KycId,
                "Document Verified",
                null,
                dto.Remarks ?? "Document verified successfully.");

            return true;
        }

        public async Task<bool> RejectDocumentAsync(int documentId, ReviewKycDto dto)
        {
            var document = await db.KycDocuments
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);

            if (document == null)
                return false;

            if (string.IsNullOrWhiteSpace(dto.Remarks))
                throw new ArgumentException(
                    "Remarks are required when rejecting a document.");

            document.VerificationStatus = "Rejected";

            await db.SaveChangesAsync();

            await AddLogAsync(
                document.KycId,
                "Document Rejected",
                null,
                dto.Remarks);

            return true;
        }

        public async Task<KycDocumentDto> ReuploadDocumentAsync(
            int documentId, UploadKycDocumentDto dto)
        {
            var document = await db.KycDocuments
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);

            if (document == null)
                throw new KeyNotFoundException("Document not found.");

            if (document.VerificationStatus != "Rejected")
                throw new InvalidOperationException(
                    "Only rejected documents can be re-uploaded.");

            var kyc = await db.CustomerKycs
                .FirstOrDefaultAsync(x => x.kyc_id == document.KycId);

            if (kyc == null)
                throw new KeyNotFoundException("KYC record not found.");

            if (kyc.verification_status == "Verified")
                throw new InvalidOperationException(
                    "Documents cannot be re-uploaded after KYC approval.");

            if (dto.KycId != document.KycId)
                throw new ArgumentException(
                    "The KYC ID does not match the original document.");

            if (dto.File == null || dto.File.Length == 0)
                throw new ArgumentException("Please select a document.");

            const long maxFileSize = 5L * 1024 * 1024;

            if (dto.File.Length > maxFileSize)
                throw new ArgumentException("Maximum file size allowed is 5 MB.");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(dto.File.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException(
                    "Only PDF, JPG, JPEG and PNG files are allowed.");

            var folder = Path.Combine(env.ContentRootPath, "SecureDocuments");
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folder, fileName);

            await using (var stream = new FileStream(
                fullPath, FileMode.CreateNew, FileAccess.Write))
            {
                await dto.File.CopyToAsync(stream);
            }

            try
            {
                document.DocumentType = dto.DocumentType;
                document.DocumentNumber = dto.DocumentNumber;
                document.FilePath = Path.Combine("SecureDocuments", fileName);
                document.UploadDate = DateTime.UtcNow;
                document.VerificationStatus = "Pending";

                await db.SaveChangesAsync();

                await AddLogAsync(
                    document.KycId,
                    "Document Re-uploaded",
                    null,
                    $"Re-uploaded document: {document.DocumentType}");

                return mapper.Map<KycDocumentDto>(document);
            }
            catch
            {
                if (File.Exists(fullPath))
                    File.Delete(fullPath);
                throw;
            }
        }
    }
}