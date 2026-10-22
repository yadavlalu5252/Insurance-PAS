using PolicyServicing.Application.DTO;
using PolicyServicing.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PolicyServicing.Application.Interface
{
    public interface IPolicyService
    {
        Task<string> ProcessAddressChangeAsync(EndorsementRequestDTO request);

        Task<List<service_request>> GetAllRequestsAsync();

        Task<service_request> GetRequestByIdAsync(long id);

        Task<bool> UpdateRequestStatusAsync(long id, string newStatus);
    }
}
