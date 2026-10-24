using Microsoft.EntityFrameworkCore;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{


    namespace PolicyServicing.Infrastructure.Repository
    {
        public class PolicyCoverageService : IProcessCoverage
        {
            private readonly AppDbContext db;

            // 🚨 Notice: PolicyClient and ProductClient are completely removed from the constructor!
            public PolicyCoverageService(AppDbContext db)
            {
                this.db = db;
            }

            public async Task<string> ProcessCoverageModificationAsync(long customerId, EndorsementRequestDTO request)
            {
                var coverageData = JsonSerializer.Deserialize<CoverageModifyDTO>(request.PayloadJson);
                if (coverageData == null) throw new Exception("Invalid payload.");

                // 1. Just package the customer's request
                var newCoveragePayload = new
                {
                    coverage_id = coverageData.CoverageId,
                    sum_insured = coverageData.NewSumInsured
                };
                string updatedValueJson = JsonSerializer.Serialize(newCoveragePayload);

                // 2. Create Service Request
                var serviceRequest = new service_request
                {
                    request_number = $"SR-{DateTime.UtcNow.Ticks}",
                    policy_id = request.PolicyId,
                    customer_id = customerId,
                    request_type = "Coverage Update", // Generic type (we don't know if it's increase/reduction without the old data)
                    request_date = DateTime.UtcNow,
                    status = RequestStatus.UnderReview
                };

                db.service_request.Add(serviceRequest);
                await db.SaveChangesAsync();

                // 3. Create Endorsement with an empty previous_value
                db.policy_endorsements.Add(new policy_endorsement
                {
                    request_id = serviceRequest.request_id,
                    endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                    endorsement_type = "Coverage Update",
                    previous_value = "{}", // The Admin side will fill this during approval
                    updated_value = updatedValueJson,
                    approval_status = Approval_status.Pending
                });

                // 4. Log History
                db.service_request_history.Add(new service_request_history
                {
                    request_id = serviceRequest.request_id,
                    action = "Coverage Update Requested (Pending Admin)",
                    action_by = customerId,
                    action_date = DateTime.UtcNow,
                    remarks = $"Requested new sum insured: {coverageData.NewSumInsured}."
                });

                await db.SaveChangesAsync();

                return $"{serviceRequest.request_number} (Pending Admin Approval)";
            }
        }
    }
}