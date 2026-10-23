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

        public ProcessCancellationService(AppDbContext db, PolicyClient client, ClaimsClient claimsClient, PremiumClient premiumClient, IMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.client = client;
            this.claimsClient = claimsClient;
            this.premiumClient = premiumClient;
            this.httpContextAccessor = httpContextAccessor;

            _mapper = mapper;
        }

        // 1. POST /api/v1/policies/{id}/cancel
        public async Task<string> ProcessCancellationAsync(long customerId, long policyId, CancellationRequestDTO request)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();

            if (string.IsNullOrWhiteSpace(request.CancellationReason))
                throw new Exception("Cancellation reason is mandatory.");

            var policyDetails = await client.GetPolicyById(policyId,token);
            if (policyDetails == null) throw new Exception("Policy not found.");

            if (policyDetails.policy_status != "Active")
                throw new Exception("Only Active policies can be cancelled.");

            // FR-SRV-008 Rule: Claims in progress prevent cancellation
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

            try {

                //    decimal refundAmount = CalculateRefund(policyDetails);

                //var payments = await premiumClient.GetPaymentHistory(policyId,token);
                //var successfulPayment = payments.FirstOrDefault(p => p.payment_status == "Success");

                //if (successfulPayment == null)
                //    throw new Exception("No successful payment found to refund for this policy.");

                //var refundPayload = new
                //{
                //    payment_id = successfulPayment.payment_id,
                //    refund_amount = refundAmount
                //};

                //bool isRefundCreated = await premiumClient.CreateRefund(refundPayload,token);
                //if (!isRefundCreated) throw new Exception("Premium Microservice failed to create the refund.");
                decimal refundAmount = CalculateRefund(policyDetails);

                bool isPolicyCancelled = await client.CancelPolicy(policyId,token);
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

        // 2. GET /api/v1/cancellations/{id}
        public async Task<CancellationDetailsDTO> GetCancellationDetailsAsync(long cancellationId)
        {
            var cancellation = await db.PolicyCancellations
                .FirstOrDefaultAsync(c => c.cancellation_id == cancellationId);

            if (cancellation == null) return null;

            // Using AutoMapper (as requested)
            return _mapper.Map<CancellationDetailsDTO>(cancellation);
        }

        private decimal CalculateRefund(PolicyDTO dto)
        {
            // Placeholder for company refund policy
            return 150.00m;
        }
    }
}