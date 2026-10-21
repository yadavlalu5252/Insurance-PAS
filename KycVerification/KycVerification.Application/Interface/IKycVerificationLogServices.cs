using KycVerification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Interface
{
    public interface IKycVerificationLogServices
    {
        Task<KycVerificationLog?> GetByIdAsync(int logId);

        Task<List<KycVerificationLog>> GetByKycIdAsync(int kycId);

        Task<List<KycVerificationLog>> GetAllAsync();

        Task<KycVerificationLog> AddAsync(KycVerificationLog log);
    }
}
