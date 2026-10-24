using AutoMapper;
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
using Microsoft.AspNetCore.Http;

namespace PolicyServicing.Infrastructure.Repository
{
    public class ProcessCancellationService : IProcessCancellation
    {
        private readonly AppDbContext db;
        private readonly PolicyClient client;
        private readonly ClaimsClient claimsClient;
        private readonly PremiumClient premiumClient;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper _mapper;

        public ProcessCancellationService(AppDbContext db, PolicyClient client, ClaimsClient claimsClient, PremiumClient premiumClient, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.client = client;
            this.claimsClient = claimsClient;
            this.premiumClient = premiumClient;
            this.httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        public async Task<string> ProcessCancellationAsync(long customerId, long policyId, CancellationRequestDTO request)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(request.CancellationReason))
                throw new Exception("Cancellation reason is mandatory.");

            var policyDetails = await client.GetPolicyById(policyId, token);
            if (policyDetails == null) throw new Exception("Policy not found.");


            if (policyDetails.customer_id != customerId)
                throw new Exception("Unauthorized: This policy does not belong to you.");

  
            if (policyDetails.policy_status != "Active" && policyDetails.policy_status != "3")
                throw new Exception("Only Active policies can be cancelled.");

            var allClaims = await claimsClient.GetAllClaims(token);
            var activeClaims = allClaims.Where(c =>
                c.policy_id == policyId &&
                c.claim_status != "Settled" &&
                c.claim_status != "Rejected").ToList();

            if (activeClaims.Any())
                throw new Exception($"Cannot cancel policy. There are {activeClaims.Count} claim(s) currently in progress.");

            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = policyId,
                customer_id = customerId,
                request_type = "Policy Cancellation",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview
            };
            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            try
            {
                // (Refund logic is still commented out for testing)
                decimal refundAmount = CalculateRefund(policyDetails);

                bool isPolicyCancelled = await client.CancelPolicy(policyId, token);
                if (!isPolicyCancelled) throw new Exception("Core Policy Microservice rejected the cancellation.");

                serviceRequest.status = RequestStatus.Completed;

                db.PolicyCancellations.Add(new PolicyCancellation
                {
                    policy_id = policyId,
                    cancellation_date = DateOnly.FromDateTime(DateTime.UtcNow),
                    cancellation_reason = request.CancellationReason,
                    refund_amount = refundAmount,
                    approved_by = request.ApprovedBy
                });

                db.policy_endorsements.Add(new policy_endorsement
                {
                    request_id = serviceRequest.request_id,
                    endorsement_number = $"CAN-{DateTime.UtcNow.Ticks}",
                    endorsement_type = "Policy Cancellation",
                    previous_value = JsonSerializer.Serialize(new { status = policyDetails.policy_status }),
                    updated_value = JsonSerializer.Serialize(new { status = "Cancelled", reason = request.CancellationReason }),
                    approval_status = Approval_status.Approved
                });

                db.service_request_history.Add(new service_request_history
                {
                    request_id = serviceRequest.request_id,
                    action = "Policy Cancelled",
                    action_by = customerId,
                    action_date = DateTime.UtcNow,
                    remarks = $"Policy cancelled. Reason: {request.CancellationReason}. Refund Initiated: {refundAmount}."
                });

                await db.SaveChangesAsync();

                return $"{serviceRequest.request_number} (Policy Cancelled Successfully. Refund Pending.)";
            }
            catch (Exception ex)
            {
                serviceRequest.status = RequestStatus.Rejected;

                db.service_request_history.Add(new service_request_history
                {
                    request_id = serviceRequest.request_id,
                    action = "Policy Cancellation Failed",
                    action_by = customerId,
                    action_date = DateTime.UtcNow,
                    remarks = ex.Message
                });

                await db.SaveChangesAsync();
                throw new Exception($"Policy Cancellation Failed: {ex.Message}");
            }
        }


        public async Task<CancellationDetailsDTO> GetCancellationDetailsAsync(long policyId, long customerId)
        {
     
            var cancellation = await db.PolicyCancellations
                .FirstOrDefaultAsync(c => c.policy_id == policyId);

            if (cancellation == null) return null;

            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var policy = await client.GetPolicyById(policyId, token);

            if (policy == null || policy.customer_id != customerId)
                return null;

        
            return new CancellationDetailsDTO
            {
                CancellationId = cancellation.cancellation_id,
                PolicyId = cancellation.policy_id,
                CancellationDate = cancellation.cancellation_date,
                CancellationReason = cancellation.cancellation_reason,
                RefundAmount = cancellation.refund_amount,
                ApprovedBy = cancellation.approved_by
            };
        }

        private decimal CalculateRefund(PolicyDTO dto)
        {
            return 150.00m; // Placeholder
        }
    }
}