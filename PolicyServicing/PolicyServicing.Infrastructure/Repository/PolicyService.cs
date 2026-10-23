using Microsoft.EntityFrameworkCore;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Config;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class PolicyService : IPolicyService
    {
        private readonly AppDbContext db;
        private readonly CustomerClient customerClient;

        // PolicyClient removed as we no longer modify the policy in the creation phase
        public PolicyService(AppDbContext db, CustomerClient customerClient)
        {
            this.db = db;
            this.customerClient = customerClient;
        }

        public async Task<List<service_request>> GetAllRequestsAsync()
        {
            return await db.service_request.OrderByDescending(r => r.request_date).ToListAsync();
        }

        public async Task<service_request> GetRequestByIdAsync(long id)
        {
            return await db.service_request.Include(r => r.policy_endorsements).FirstOrDefaultAsync(r => r.request_id == id);
        }

        public async Task<string> ProcessAddressChangeAsync(EndorsementRequestDTO request)
        {
            var addressChange = JsonSerializer.Deserialize<AddressChangeDTO>(request.PayloadJson);

            // 1. Business Rule Validation (FR-SRV-002)
            if (addressChange == null || string.IsNullOrEmpty(addressChange.AddressProofDocumentPath))
            {
                throw new Exception("Address proof document is mandatory");
            }

            // Read old address for the audit trail
            var allAddresses = await customerClient.GetCustomerAddresses(request.CustomerId);
            var oldAddress = allAddresses.FirstOrDefault(a => a.address_type == addressChange.AddressType);

            if (oldAddress == null)
            {
                throw new Exception("Existing address not found");
            }

            string previousValueJson = JsonSerializer.Serialize(oldAddress);

            var newAddressData = new CustomerAddressDTO
            {
                customer_id = request.CustomerId,
                address_type = addressChange.AddressType,
                address_line1 = addressChange.HouseNo,
                address_line2 = addressChange.Street,
                city = addressChange.City,
                district = addressChange.District,
                state = addressChange.State,
                country = addressChange.Country,
                pincode = addressChange.PinCode
            };

            string updatedValueJson = JsonSerializer.Serialize(newAddressData);

            // 2. Create Service Request (Pending Admin Approval)
            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = request.PolicyId,
                customer_id = request.CustomerId,
                request_type = "Address Change",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview // Changed to UnderReview
            };

            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            // 3. Create Endorsement (Pending)
            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = "Address Change",
                previous_value = previousValueJson,
                updated_value = updatedValueJson,
                approval_status = Approval_status.Pending // Changed to Pending
            });

            // 4. Log History
            db.service_request_history.Add(new service_request_history
            {
                request_id = serviceRequest.request_id,
                action = "Address Change Requested (Pending Admin)",
                action_by = request.CustomerId,
                action_date = DateTime.UtcNow,
                remarks = $"Proof Document : {addressChange.AddressProofDocumentPath}"
            });

            await db.SaveChangesAsync();

            return $"{serviceRequest.request_number} (Pending Admin Approval)";
        }

        public async Task<bool> UpdateRequestStatusAsync(long id, string newStatus)
        {
            var request = await db.service_request.FindAsync(id);
            if (request == null) return false;

            if (Enum.TryParse<RequestStatus>(newStatus, true, out var parsedStatus))
            {
                request.status = parsedStatus;
                await db.SaveChangesAsync();
                return true;
            }

            return false;
        }
    }
}