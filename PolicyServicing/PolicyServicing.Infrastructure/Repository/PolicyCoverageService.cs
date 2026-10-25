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


            public PolicyCoverageService(AppDbContext db)
            {
                this.db = db;
            }

            public async Task<string> ProcessCoverageModificationAsync(long customerId, EndorsementRequestDTO request)
            {
                var coverageData = JsonSerializer.Deserialize<CoverageModifyDTO>(request.PayloadJson);
                if (coverageData == null) throw new Exception("Invalid payload.");


                var newCoveragePayload = new
                {
                    coverage_id = coverageData.CoverageId,
                    sum_insured = coverageData.NewSumInsured
                };
                string updatedValueJson = JsonSerializer.Serialize(newCoveragePayload);


                var serviceRequest = new service_request
                {
                    request_number = $"SR-{DateTime.UtcNow.Ticks}",
                    policy_id = request.PolicyId,
                    customer_id = customerId,
                    request_type = "Coverage Update", 
                    request_date = DateTime.UtcNow,
                    status = RequestStatus.UnderReview
                };

                db.service_request.Add(serviceRequest);
                await db.SaveChangesAsync();


                db.policy_endorsements.Add(new policy_endorsement
                {
                    request_id = serviceRequest.request_id,
                    endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                    endorsement_type = "Coverage Update",
                    previous_value = "{}", 
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