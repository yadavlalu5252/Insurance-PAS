using KycVerification.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Interface
{
    public interface IKycDocumentServices
    {
        Task<KycDocument?> GetByIdAsync(int documentId);
        Task<List<KycDocument>> GetByKycIdAsync(int kycId);
        Task<KycDocument> AddAsync(KycDocument document);
        Task UpdateAsync(KycDocument document);

        Task<KycDocument> UploadAsync(int kycId, string documentType,string documentNumber, IFormFile file);
    }
}
