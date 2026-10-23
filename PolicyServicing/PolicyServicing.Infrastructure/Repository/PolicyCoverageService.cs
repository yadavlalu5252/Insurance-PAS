using Microsoft.EntityFrameworkCore;
using PolicyServicing.Application.DTO;
using PolicyServicing.Application.Interface;
using PolicyServicing.Domain.Entities;
using PolicyServicing.Infrastructure.Config;
using PolicyServicing.Infrastructure.Data;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PolicyServicing.Infrastructure.Repository
{
    public class PolicyCoverageService : IProcessCoverage
    {
        private readonly AppDbContext db;
        private readonly PolicyClient client;
        private readonly ProductClient productClient; // <-- 1. Inject ProductClient

        public PolicyCoverageService(AppDbContext db, PolicyClient client, ProductClient productClient)
        {
            this.db = db;
            this.client = client;
            this.productClient = productClient;
        }

        public async Task<string> ProcessCoverageModificationAsync(long customerId, EndorsementRequestDTO request)
        {
            var coverageData = JsonSerializer.Deserialize<CoverageModifyDTO>(request.PayloadJson);
            if (coverageData == null) throw new Exception("Invalid payload.");

            // Read policy details to validate and get the previous value
            var policyDetails = await client.GetPolicyById(request.PolicyId);
            if (policyDetails == null) throw new Exception("Policy not found.");

            var coverages = await client.GetPolicyCoverages(request.PolicyId);
            var oldCoverage = coverages.FirstOrDefault(c => c.coverage_id == coverageData.CoverageId);
            if (oldCoverage == null) throw new Exception("Coverage not found on this policy.");

           
            var productCoverages = await productClient.GetProductCoverages(policyDetails.product_id);
            var coverageRules = productCoverages.FirstOrDefault(c => c.coverage_id == coverageData.CoverageId);

            if (coverageRules == null)
                throw new Exception("Coverage rules not found in Product Microservice.");

            if (coverageData.NewSumInsured < coverageRules.minimum_coverage ||
                coverageData.NewSumInsured > coverageRules.maximum_coverage)
            {
                throw new Exception($"Sum insured must be between {coverageRules.minimum_coverage} and {coverageRules.maximum_coverage} for this coverage.");
            }

            string previousValueJson = JsonSerializer.Serialize(oldCoverage);

            var newCoveragePayload = new
            {
                coverage_id = coverageData.CoverageId,
                sum_insured = coverageData.NewSumInsured
            };

            string updatedValueJson = JsonSerializer.Serialize(newCoveragePayload);

            bool isIncrease = coverageData.NewSumInsured > oldCoverage.sum_insured;
            string requestType = isIncrease ? "Coverage Increase" : "Coverage Reduction";

            
            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = request.PolicyId,
                customer_id = customerId,
                request_type = requestType,
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview
            };

            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            db.policy_endorsements.Add(new policy_endorsement
            {
                request_id = serviceRequest.request_id,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = requestType,
                previous_value = previousValueJson,
                updated_value = updatedValueJson,
                approval_status = Approval_status.Pending
            });

     
            db.service_request_history.Add(new service_request_history
            {
                request_id = serviceRequest.request_id,
                action = $"{requestType} Requested (Pending Admin)",
                action_by = customerId,
                action_date = DateTime.UtcNow,
                remarks = $"Coverage changed from {oldCoverage.sum_insured} to {coverageData.NewSumInsured}."
            });

            await db.SaveChangesAsync();

            return $"{serviceRequest.request_number} (Pending Admin Approval)";
        }
    }
}