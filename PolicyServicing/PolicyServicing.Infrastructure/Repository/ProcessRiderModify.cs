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


        public ProcessRiderModify(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<string> ProcessRiderModificationAsync(long customerId, EndorsementRequestDTO request)
        {
            var riderData = JsonSerializer.Deserialize<RiderModifyDTO>(request.PayloadJson);
            if (riderData == null) throw new Exception("Invalid payload.");


            if (riderData.Action != "Add" && riderData.Action != "Remove")
                throw new Exception("Invalid Action. Must be 'Add' or 'Remove'.");


            string updatedValueJson = JsonSerializer.Serialize(riderData);


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

            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = riderData.Action == "Add" ? "Rider Addition" : "Rider Removal",
                previous_value = "{}", 
                updated_value = updatedValueJson, 
                approval_status = Approval_status.Pending
            });

   
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