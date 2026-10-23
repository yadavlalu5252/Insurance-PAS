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
    public class ProcessRiderModify : IProcessRiderModify
    {
        private readonly AppDbContext db;
        private readonly PolicyClient client;
        private readonly ProductClient productClient;

        // Removed PremiumClient from constructor
        public ProcessRiderModify(AppDbContext db, PolicyClient client, ProductClient productClient)
        {
            this.db = db;
            this.client = client;
            this.productClient = productClient;
        }

        public async Task<string> ProcessRiderModificationAsync(long customerId, EndorsementRequestDTO request)
        {
            var riderData = JsonSerializer.Deserialize<RiderModifyDTO>(request.PayloadJson);
            if (riderData == null) throw new Exception("Invalid payload.");

            // Read Policy Details (External API call for validation)
            var policyDetails = await client.GetPolicyById(request.PolicyId);
            if (policyDetails == null) throw new Exception("Policy not found.");
            if (policyDetails.policy_status != "Active")
                throw new Exception("Policy must be Active to modify riders.");

            // Read Eligible Riders (External API call for validation)
            var eligibleRiders = await productClient.GetEligibleRiders(policyDetails.product_id);
            var requestedRider = eligibleRiders.FirstOrDefault(r => r.rider_id == riderData.RiderId);

            if (requestedRider == null)
                throw new Exception("This rider is not eligible for your policy's product.");

            var oldRidersList = policyDetails.riders;
            string previousValueJson = JsonSerializer.Serialize(oldRidersList);

            var updatedRidersList = new List<PolicyRiderDTO>(oldRidersList);

            if (riderData.Action == "Add")
            {
                if (updatedRidersList.Any(r => r.rider_id == riderData.RiderId))
                    throw new Exception("Policy already has this rider.");

                updatedRidersList.Add(new PolicyRiderDTO { rider_id = riderData.RiderId });
            }
            else if (riderData.Action == "Remove")
            {
                var riderToRemove = updatedRidersList.FirstOrDefault(r => r.rider_id == riderData.RiderId);
                if (riderToRemove == null)
                    throw new Exception("Rider not found on this policy.");

                updatedRidersList.Remove(riderToRemove);
            }
            else
            {
                throw new Exception("Invalid Action. Must be 'Add' or 'Remove'.");
            }

            string updatedValueJson = JsonSerializer.Serialize(updatedRidersList);

            // Create Service Request (Pending Admin Approval)
            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = request.PolicyId,
                customer_id = customerId,
                request_type = riderData.Action == "Add" ? "Rider Addition" : "Rider Removal",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview // Changed to UnderReview
            };

            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            // Create Endorsement (Pending)
            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = riderData.Action == "Add" ? "Rider Addition" : "Rider Removal",
                previous_value = previousValueJson,
                updated_value = updatedValueJson,
                approval_status = Approval_status.Pending // Changed to Pending
            });

            // Log History
            db.service_request_history.Add(new service_request_history
            {
                request_id = serviceRequest.request_id,
                action = $"{riderData.Action} Rider Requested (Pending Admin)",
                action_by = customerId,
                action_date = DateTime.UtcNow,
                remarks = $"RiderId: {riderData.RiderId} requested for modification."
            });

            await db.SaveChangesAsync();

            // Note: The return message is updated to reflect it is pending
            return $"{serviceRequest.request_number} (Pending Admin Approval)";
        }
    }
}