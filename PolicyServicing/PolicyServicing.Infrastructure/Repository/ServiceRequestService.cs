using Microsoft.EntityFrameworkCore;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly AppDbContext db;

        // Inject all the validation services to route the POST request
        private readonly IPolicyService _addressChangeService;
        private readonly IProcessNominee _nomineeService;
        private readonly IProcessCoverage _coverageService;
        private readonly IProcessRiderModify _riderService;
        private readonly IUpdateContact _contactService;

        public ServiceRequestService(
            AppDbContext db,
            IPolicyService addressChangeService,
            IProcessNominee nomineeService,
            IProcessCoverage coverageService,
            IProcessRiderModify riderService,
            IUpdateContact contactService)
        {
            this.db = db;
            _addressChangeService = addressChangeService;
            _nomineeService = nomineeService;
            _coverageService = coverageService;
            _riderService = riderService;
            _contactService = contactService;
        }

        // 1. POST /api/v1/service-requests
        public async Task<string> CreateServiceRequestAsync(EndorsementRequestDTO request)
        {
            // Routes to the specific service for validation and saving to DB
            return request.EndorsementType switch
            {
                "Address Change" => await _addressChangeService.ProcessAddressChangeAsync(request),
                "Nominee Change" => await _nomineeService.ProcessNomineeChangeAsync(request.CustomerId, request),
                "Coverage Increase" or "Coverage Reduction" => await _coverageService.ProcessCoverageModificationAsync(request.CustomerId, request),
                "Rider Addition" or "Rider Removal" => await _riderService.ProcessRiderModificationAsync(request.CustomerId, request),
                "Contact Update" => await _contactService.ProcessContactUpdateAsync(request),
                _ => throw new Exception($"Invalid Endorsement Type: '{request.EndorsementType}'.")
            };
        }

        // 2. GET /api/v1/service-requests
        public async Task<List<service_request>> ListRequestsAsync()
        {
            return await db.service_request
                .OrderByDescending(r => r.request_date)
                .ToListAsync();
        }

        // 3. GET /api/v1/service-requests/{id}
        public async Task<service_request> ViewRequestAsync(long id)
        {
            return await db.service_request
                .Include(r => r.policy_endorsements)
                .FirstOrDefaultAsync(r => r.request_id == id);
        }

        // 4. PUT /api/v1/service-requests/{id}
        public async Task<bool> UpdateRequestAsync(long id, string newStatus, long adminId)
        {
            var request = await db.service_request.FindAsync(id);
            if (request == null) return false;

            if (Enum.TryParse<RequestStatus>(newStatus, true, out var parsedStatus))
            {
                request.status = parsedStatus;

                // Log the status change in history
                db.service_request_history.Add(new service_request_history
                {
                    request_id = request.request_id,
                    action = $"Status manually updated to {parsedStatus} by Admin",
                    action_by = adminId,
                    action_date = DateTime.UtcNow,
                    remarks = "Manual status override."
                });

                await db.SaveChangesAsync();
                return true;
            }

            return false; // Invalid enum string
        }
    }
}