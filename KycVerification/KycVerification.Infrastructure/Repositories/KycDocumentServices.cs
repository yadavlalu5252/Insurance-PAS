using AutoMapper;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using KycVerification.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace KycVerification.Infrastructure.Repositories
{
    public class KycDocumentServices : IKycDocumentServices
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;
        private readonly IWebHostEnvironment env;

        public KycDocumentServices(AppDbContext db, IMapper mapper, IWebHostEnvironment env)
        {
            this.db = db;
            this.mapper = mapper;
            this.env = env;
        }

        public async Task<KycDocument?> GetByIdAsync(int documentId)
        {
            return await db.KycDocuments
                .FirstOrDefaultAsync(x => x.DocumentId == documentId);
        }

        public async Task<List<KycDocument>> GetByKycIdAsync(int kycId)
        {
            return await db.KycDocuments
                .Where(x => x.KycId == kycId)
                .ToListAsync();
        }

        public async Task<KycDocument> AddAsync(KycDocument document)
        {
            document.DocumentId = 0;
            document.UploadDate = DateTime.UtcNow;
            document.VerificationStatus = "Pending";

            await db.KycDocuments.AddAsync(document);
            await db.SaveChangesAsync();

            return document;
        }

        public async Task UpdateAsync(KycDocument document)
        {
            var data = await db.KycDocuments
                .FirstOrDefaultAsync(x => x.DocumentId == document.DocumentId);

            if (data == null)
                throw new KeyNotFoundException(
                    $"KYC document with ID {document.DocumentId} was not found.");

            data.DocumentType = document.DocumentType;
            data.DocumentNumber = document.DocumentNumber;
            data.FilePath = document.FilePath;
            data.VerificationStatus = document.VerificationStatus;

            await db.SaveChangesAsync();
        }

        // ✅ Matches interface exactly: Task<KycDocument?>, long, string?, string?
        public async Task<KycDocument?> UploadAsync(
            long kycId,
            string? documentType,
            string? documentNumber,
            IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Please select a document.");

            const long maxFileSize = 5 * 1024 * 1024;
            if (file.Length > maxFileSize)
                throw new ArgumentException("Maximum file size is 5 MB.");

            var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
                throw new ArgumentException("Only PDF, JPG, JPEG and PNG files are allowed.");

            var kycIdInt = (int)kycId;
            var kycExists = await db.CustomerKycs.AnyAsync(x => x.kyc_id == kycIdInt); if (!kycExists)
                throw new KeyNotFoundException("KYC record not found.");

            var folderPath = Path.Combine(env.ContentRootPath, "SecureDocuments");
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(folderPath, fileName);

            try
            {
                await using (var stream = new FileStream(fullPath, FileMode.CreateNew, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new KycDocument
                {
                    KycId = kycIdInt,
                    DocumentType = documentType,
                    DocumentNumber = documentNumber,
                    FilePath = Path.Combine("SecureDocuments", fileName),
                    UploadDate = DateTime.UtcNow,
                    VerificationStatus = "Pending"
                };

                await db.KycDocuments.AddAsync(document);
                await db.SaveChangesAsync();

                return document;
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