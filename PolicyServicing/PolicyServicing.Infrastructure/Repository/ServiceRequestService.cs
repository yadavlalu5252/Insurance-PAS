using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Config;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class ServiceRequestService : IServiceRequestService
    {
        private readonly AppDbContext db;


        private readonly IPolicyService addressChangeService;
        private readonly IProcessNominee nomineeService;
        private readonly IProcessCoverage coverageService;
        private readonly IProcessRiderModify riderService;
        private readonly IUpdateContact contactService;

        private readonly PolicyClient policyClient;

        private readonly IHttpContextAccessor _httpContextAccessor;




        public ServiceRequestService(
            AppDbContext db,
            IPolicyService addressChangeService,
            IProcessNominee nomineeService,
            IProcessCoverage coverageService,
            IProcessRiderModify riderService,
            IUpdateContact contactService,PolicyClient policyClient,IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.addressChangeService = addressChangeService;
            this.nomineeService = nomineeService;
            this.coverageService = coverageService;
            this.riderService = riderService;
            this.contactService = contactService;
            this.policyClient = policyClient;
            this._httpContextAccessor = httpContextAccessor;
        }


        public async Task<string> CreateServiceRequestAsync(EndorsementRequestDTO request)
        {

            return request.EndorsementType switch
            {
                "Address Change" => await addressChangeService.ProcessAddressChangeAsync(request),
                "Nominee Change" => await nomineeService.ProcessNomineeChangeAsync(request.CustomerId, request),
                "Coverage Increase" or "Coverage Reduction" => await coverageService.ProcessCoverageModificationAsync(request.CustomerId, request),
                "Rider Addition" or "Rider Removal" => await riderService.ProcessRiderModificationAsync(request.CustomerId, request),
                "Contact Update" => await contactService.ProcessContactUpdateAsync(request),
                _ => throw new Exception($"Invalid Endorsement Type: '{request.EndorsementType}'.")
            };
        }


        public async Task<List<service_request>> ListRequestsAsync()
        {
            return await db.service_request
                .OrderByDescending(r => r.request_date)
                .ToListAsync();
        }

        public async Task<service_request?> ViewRequestAsync(long id)
        {
            return await db.service_request
                .Include(r => r.policy_endorsements)
                .FirstOrDefaultAsync(r => r.request_id == id);
        }

        public async Task<bool> UpdateRequestAsync(long id, string newStatus, long adminId)
        {
            var request = await db.service_request.FindAsync(id);
            if (request == null) return false;

            if (Enum.TryParse<RequestStatus>(newStatus, true, out var parsedStatus))
            {
                request.status = parsedStatus;

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

            return false; 
        }

        public async Task<List<PolicyDTO>> GetCustomerPoliciesAsync(long customerId)
        {
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            return await policyClient.GetPoliciesByCustomerId(customerId, token);
        }
    }
}