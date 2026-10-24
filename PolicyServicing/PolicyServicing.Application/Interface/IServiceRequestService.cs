using PolicyServicing.Application.DTO;
using PolicyServicing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.Interface
{
    public interface IServiceRequestService
    {
        Task<string> CreateServiceRequestAsync(EndorsementRequestDTO request);
        Task<List<service_request>> ListRequestsAsync();
        Task<service_request?> ViewRequestAsync(long id);
        Task<bool> UpdateRequestAsync(long id, string newStatus, long adminId);

        Task<List<PolicyDTO>> GetCustomerPoliciesAsync(long customerId, string token);
    }
}
