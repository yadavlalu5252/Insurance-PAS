using KycVerification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Interface
{
    public interface ICustomerKycServices
    {
        Task<CustomerKyc?> GetByIdAsync(int kycId);
        Task<CustomerKyc?> GetByCustomerIdAsync(int customerId);
        Task<List<CustomerKyc>> GetAllAsync();
        Task<CustomerKyc> AddAsync(CustomerKyc kyc);
        Task UpdateAsync(CustomerKyc kyc);
    }
}
