
using AutoMapper;
using CustomerRegistration.Application.DTOs.Document;
using CustomerRegistration.Application.Interfaces;
using CustomerRegistration.Domain.Entities;
using CustomerRegistration.Infrastructure.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CustomerRegistration.Infrastructure.Services;

public class CustomerDocumentService : ICustomerDocumentService
{
    private readonly AppDbContext db;
    private readonly IMapper mapper;
    private readonly IWebHostEnvironment env;
    private readonly IConfiguration cnf;

    private readonly string[] _allowedExtensions =
        { ".pdf", ".jpg", ".jpeg", ".png" };

    private const long MaxFileSize = 5 * 1024 * 1024;

    public CustomerDocumentService(AppDbContext db,IMapper mapper,IWebHostEnvironment environment,IConfiguration configuration)
    {
        this.db = db;
        this.mapper = mapper;
        this.env = environment;
        this.cnf = configuration;
    }

    public async Task<CustomerDocumentDto?> UploadDocument(long customerId, UploadCustomerDocumentDto dto)
    {
        if (dto.file == null || dto.file.Length == 0)
            throw new InvalidOperationException("Please select a document.");

        if (dto.file.Length > MaxFileSize)
            throw new InvalidOperationException("Maximum file size is 5 MB.");

        var extension = Path.GetExtension(dto.file.FileName).ToLowerInvariant();

        if (!_allowedExtensions.Contains(extension))
            throw new InvalidOperationException(
                "Only PDF, JPG, JPEG and PNG files are allowed.");

        var customerExists = await db.customers
            .AnyAsync(c => c.customer_id == customerId);

        if (!customerExists)
            return null;

        var storagePath = cnf["CustomerDocuments:StoragePath"];

        if (string.IsNullOrWhiteSpace(storagePath))
        {
            storagePath = Path.Combine(
                env.ContentRootPath,
                "App_Data",
                "CustomerDocuments");
        }
        else if (!Path.IsPathRooted(storagePath))
        {
            storagePath = Path.Combine(
                env.ContentRootPath, storagePath);
        }

        Directory.CreateDirectory(storagePath);

        var storedFileName = $"{Guid.NewGuid():N}{extension}";
        var fullPath = Path.Combine(storagePath, storedFileName);

        try
        {
            await using (var stream = new FileStream(
                fullPath, FileMode.CreateNew, FileAccess.Write))
            {
                await dto.file.CopyToAsync(stream);
            }

            var document = new CustomerDocument
            {
                customer_id = customerId,
                document_type = dto.document_type,
                file_name = Path.GetFileName(dto.file.FileName),
                file_path = fullPath,
                uploaded_date = DateTime.UtcNow,
                verification_status = "Pending"
            };

            db.customer_documents.Add(document);
            await db.SaveChangesAsync();

            return mapper.Map<CustomerDocumentDto>(document);
        }
        catch
        {
            if (File.Exists(fullPath))
                File.Delete(fullPath);

            throw;
        }
    }

    public async Task<List<CustomerDocumentDto>> GetDocuments(long customerId)
    {
        var documents = await db.customer_documents
            .AsNoTracking()
            .Where(d => d.customer_id == customerId)
            .OrderByDescending(d => d.uploaded_date)
            .ToListAsync();

        return mapper.Map<List<CustomerDocumentDto>>(documents);
    }

    public async Task<bool> DeleteDocument(long documentId)
    {
        var document = await db.customer_documents
            .FirstOrDefaultAsync(d => d.document_id == documentId);

        if (document == null)
            return false;

        var filePath = document.file_path;

        db.customer_documents.Remove(document);
        await db.SaveChangesAsync();

        if (File.Exists(filePath))
            File.Delete(filePath);

        return true;
    }

    public async Task<CustomerDocumentDto?> VerifyDocument(
        long documentId, VerifyCustomerDocumentDto dto)
    {
        var document = await db.customer_documents
            .FirstOrDefaultAsync(d => d.document_id == documentId);

        if (document == null)
            return null;

        if (dto.verification_status != "Verified" &&
            dto.verification_status != "Rejected")
        {
            throw new InvalidOperationException(
                "Verification status must be Verified or Rejected.");
        }

        document.verification_status = dto.verification_status;

        await db.SaveChangesAsync();

        return mapper.Map<CustomerDocumentDto>(document);
    }
}
