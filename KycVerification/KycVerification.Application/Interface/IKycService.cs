using KycVerification.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Interface
{
    public interface IKycService
    {
        Task<CustomerKycDto> CreateKycAsync( CreateCustomerKycDto dto);

        Task<CustomerKycDto?> GetKycByIdAsync(int kycId);

        Task<List<CustomerKycDto>> GetAllKycAsync();

        Task<KycDocumentDto> UploadDocumentAsync( UploadKycDocumentDto dto);

        Task<bool> ApproveKycAsync(int kycId, ReviewKycDto dto);

        Task<bool> RejectKycAsync(int kycId, ReviewKycDto dto);

        Task<List<KycDocumentDto>> GetDocumentsAsync(int kycId);

        Task<bool> VerifyDocumentAsync(int documentId, ReviewKycDto dto);

        Task<bool> RejectDocumentAsync(int documentId, ReviewKycDto dto);

        Task<KycDocumentDto> ReuploadDocumentAsync( int documentId, UploadKycDocumentDto dto);
    }
}
