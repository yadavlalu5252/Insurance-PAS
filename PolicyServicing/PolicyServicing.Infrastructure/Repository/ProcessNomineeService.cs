using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class ProcessNomineeService : IProcessNominee
    {
        private readonly AppDbContext db;


        public ProcessNomineeService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<string> ProcessNomineeChangeAsync(long customerId, EndorsementRequestDTO request)
        {
            var nomineeData = JsonSerializer.Deserialize<NomineeUpdateDTO>(request.PayloadJson);

            // 1. Business Rule Validation (FR-SRV-004)
            decimal totalShare = nomineeData.Nominees.Sum(n => n.share_percentage);
            if (totalShare != 100.00m)
            {
                throw new Exception($"Total nominee share must equal 100%. Currently it is {totalShare}%.");
            }

            DateTime adultCutoffDate = DateTime.Today.AddYears(-18);
            foreach (var nominee in nomineeData.Nominees)
            {
                if (nominee.date_of_birth > adultCutoffDate && string.IsNullOrEmpty(nominee.guardian_name))
                {
                    throw new Exception($"Nominee {nominee.nominee_name} is a minor. Guardian name is mandatory.");
                }
            }

            string updatedValueJson = JsonSerializer.Serialize(nomineeData.Nominees);

            // 2. Create Service Request (Pending Admin Approval)
            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = request.PolicyId,
                customer_id = customerId,
                request_type = "Nominee Change",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview
            };

            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            // 3. Create Endorsement (Pending)
            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = "Nominee Change",
                previous_value = "{}", 
                updated_value = updatedValueJson,
                approval_status = Approval_status.Pending
            });

            // 4. Log History
            db.service_request_history.Add(new service_request_history
            {
                request_id = serviceRequest.request_id,
                action = "Nominee Change Requested (Pending Admin)",
                action_by = customerId,
                action_date = DateTime.UtcNow,
                remarks = $"Requested update for {nomineeData.Nominees.Count} nominees. Total share: 100%."
            });

            await db.SaveChangesAsync();

            return $"{serviceRequest.request_number} (Pending Admin Approval)";
        }
    }
}