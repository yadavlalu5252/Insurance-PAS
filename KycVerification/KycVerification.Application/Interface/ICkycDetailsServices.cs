using KycVerification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Interface
{
    public interface ICkycDetailsServices
    {
       
        Task<CkycDetails?> GetByIdAsync(int ckycId);

        Task<CkycDetails?> GetByCustomerIdAsync(int customerId);

        Task<List<CkycDetails>> GetAllAsync();

        Task<CkycDetails> AddAsync(CkycDetails details);

        Task UpdateAsync(CkycDetails details);
    }
}
