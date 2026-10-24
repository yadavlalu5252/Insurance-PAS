using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class ProcessRiderModify : IProcessRiderModify
    {
        private readonly AppDbContext db;

        // 🚨 PolicyClient and ProductClient completely removed!
        public ProcessRiderModify(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<string> ProcessRiderModificationAsync(long customerId, EndorsementRequestDTO request)
        {
            var riderData = JsonSerializer.Deserialize<RiderModifyDTO>(request.PayloadJson);
            if (riderData == null) throw new Exception("Invalid payload.");

            // Basic validation (no external API needed)
            if (riderData.Action != "Add" && riderData.Action != "Remove")
                throw new Exception("Invalid Action. Must be 'Add' or 'Remove'.");

            // Save the intent as updated_value (The Admin side will compute the actual list)
            string updatedValueJson = JsonSerializer.Serialize(riderData);

            // Create Service Request (Pending Admin Approval)
            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = request.PolicyId,
                customer_id = customerId,
                request_type = riderData.Action == "Add" ? "Rider Addition" : "Rider Removal",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview
            };

            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            // Create Endorsement (Pending)
            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = riderData.Action == "Add" ? "Rider Addition" : "Rider Removal",
                previous_value = "{}", // 🚨 Left blank, Admin will fill it
                updated_value = updatedValueJson, // Contains RiderId and Action
                approval_status = Approval_status.Pending
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

            return $"{serviceRequest.request_number} (Pending Admin Approval)";
        }
    }
}