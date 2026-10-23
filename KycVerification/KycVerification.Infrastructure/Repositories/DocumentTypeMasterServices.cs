using AutoMapper;
using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using KycVerification.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Infrastructure.Repositories
{
    public class DocumentTypeMasterServices : IDocumentTypeMasterServices
    {
        private readonly AppDbContext db;
        private readonly IMapper mapper;

        public DocumentTypeMasterServices( AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        // Get document type by ID
        public async Task<DocumentTypeMaster?> GetByIdAsync(
            int documentTypeId)
        {
            return await db.DocumentTypeMasters
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    x => x.DocumentTypeId == documentTypeId);
        }

        // Get all document types
        public async Task<List<DocumentTypeMaster>> GetAllAsync()
        {
            return await db.DocumentTypeMasters
                .AsNoTracking()
                .OrderBy(x => x.DocumentName)
                .ToListAsync();
        }

        // Add a new document type
        public async Task<DocumentTypeMaster> AddAsync(
            DocumentTypeMaster documentType)
        {
            var map = mapper.Map<DocumentTypeMaster>(documentType);

            map.DocumentTypeId = 0;

            await db.DocumentTypeMasters.AddAsync(map);
            await db.SaveChangesAsync();

            return map;
        }

        // Update an existing document type
        public async Task UpdateAsync(
            DocumentTypeMaster documentType)
        {
            var data = await db.DocumentTypeMasters
                .FirstOrDefaultAsync(
                    x => x.DocumentTypeId == documentType.DocumentTypeId);

            if (data == null)
            {
                throw new KeyNotFoundException(
                    $"Document type with ID {documentType.DocumentTypeId} was not found.");
            }

            data.DocumentName = documentType.DocumentName;
            data.MandatoryFlag = documentType.MandatoryFlag;
            data.Status = documentType.Status;

            await db.SaveChangesAsync();
        }
    }
}
