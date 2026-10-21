using KycVerification.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KycVerification.Application.Interface
{
    public interface IDocumentTypeMasterServices
    {
        Task<DocumentTypeMaster?> GetByIdAsync(int id);
        Task<List<DocumentTypeMaster>> GetAllAsync();
        Task<DocumentTypeMaster> AddAsync(DocumentTypeMaster documentType);
        Task UpdateAsync(DocumentTypeMaster documentType);
    }
}
