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
    public class PolicyRenewalService : IPolicyRenew
    {
        private readonly AppDbContext db;
        private readonly PolicyClient client;
        private readonly PremiumClient premiumClient;

        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMapper mapper;

        public PolicyRenewalService(AppDbContext db, PolicyClient client, PremiumClient premiumClient, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            this.db = db;
            this.client = client;
            this.premiumClient = premiumClient;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
        }


        public async Task<string> ProcessPolicyRenewalAsync(long customerId, long policyId, RenewalRequestDTO request)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var policyDetails = await client.GetPolicyById(policyId, token);
            if (policyDetails == null) throw new Exception("Policy not found.");
            if (policyDetails.policy_status == "Cancelled" || policyDetails.policy_status == "Closed")
                throw new Exception("Cancelled or Closed policies cannot be renewed.");

            var schedules = await premiumClient.GetPremiumSchedule(policyId, token);
            var unpaidInstallments = schedules.Where(s => s.status != "Paid" && s.status != "Cancelled");
            if (unpaidInstallments.Any())
                throw new Exception("Cannot renew policy. There are pending premium payments.");

            var serviceRequest = new service_request
            {
                request_number = $"SR-{DateTime.UtcNow.Ticks}",
                policy_id = policyId,
                customer_id = customerId,
                request_type = "Policy Renewal",
                request_date = DateTime.UtcNow,
                status = RequestStatus.UnderReview
            };
            db.service_request.Add(serviceRequest);
            await db.SaveChangesAsync();

            try
            {
 
                var premiumPayload = new
                {
                    policy_id = (int)policyId,
                    issue_date = policyDetails.issue_date,
                    effective_date = policyDetails.expiry_date, 
                    expiry_date = request.NewExpiryDate,
                    premium_frequency = "Monthly",
                    total_premium = policyDetails.premium_amount 
                };

                bool isGenerated = await premiumClient.GeneratePremiumSchedule(premiumPayload, token);
                if (!isGenerated) throw new Exception("Failed to generate renewal premium schedule.");

                var newSchedules = await premiumClient.GetPremiumSchedule(policyId, token);
                decimal renewalPremiumAmount = newSchedules.Sum(s => s.premium_amount);

                if (renewalPremiumAmount <= 0)
                    throw new Exception("Invalid premium amount calculated by the Premium Microservice.");

                policyDetails.expiry_date = request.NewExpiryDate;

                bool isPolicyUpdated = await client.RenewPolicy(policyId, request.NewExpiryDate, token);
                if (!isPolicyUpdated) throw new Exception("Core Policy Microservice rejected the renewal.");


                serviceRequest.status = RequestStatus.Completed;

                var renewalRecord = new policy_renewal
                {
                    policy_id = policyId,
                    renewal_date = DateOnly.FromDateTime(DateTime.UtcNow),
                    new_expiry_date = DateOnly.FromDateTime(request.NewExpiryDate),
                    premium_amount = renewalPremiumAmount,
                    Renewal_Status = renewal_status.Renewed
                };
                db.policy_renewal.Add(renewalRecord);

                db.policy_endorsements.Add(new policy_endorsement
                {
                    request_id = serviceRequest.request_id,
                    endorsement_number = $"REN-{DateTime.UtcNow.Ticks}",
                    endorsement_type = "Policy Renewal",
                    previous_value = JsonSerializer.Serialize(new { expiry_date = policyDetails.expiry_date }),
                    updated_value = JsonSerializer.Serialize(new { expiry_date = request.NewExpiryDate }),
                    approval_status = Approval_status.Approved
                });

                db.service_request_history.Add(new service_request_history
                {
                    request_id = serviceRequest.request_id,
                    action = "Policy Renewed",
                    action_by = customerId,
                    action_date = DateTime.UtcNow,
                    remarks = $"Policy renewed. New expiry: {request.NewExpiryDate:yyyy-MM-dd}."
                });

                await db.SaveChangesAsync();
                return $"{serviceRequest.request_number} (Policy Renewed Successfully)";
            }
            catch (Exception ex)
            {
                serviceRequest.status = RequestStatus.Rejected;
                db.service_request_history.Add(new service_request_history
                {
                    request_id = serviceRequest.request_id,
                    action = "Policy Renewal Failed",
                    action_by = customerId,
                    action_date = DateTime.UtcNow,
                    remarks = ex.Message
                });

                await db.SaveChangesAsync();
                throw new Exception($"Policy Renewal Failed: {ex.Message}");
            }
        }


        public async Task<RenewalDetailsResponseDTO> GetRenewalDetailsAsync(long policyId, long customerId)
        {
  
            var renewal = await db.policy_renewal
                .Where(r => r.policy_id == policyId)
                .OrderByDescending(r => r.renewal_id) 
                .FirstOrDefaultAsync();

            if (renewal == null) return null;


            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var policy = await client.GetPolicyById(policyId, token);

            if (policy == null || policy.customer_id != customerId)
            {
                return null; 
            }


            return new RenewalDetailsResponseDTO
            {
                RenewalId = renewal.renewal_id,
                PolicyId = renewal.policy_id,
                RenewalDate = renewal.renewal_date,
                NewExpiryDate = renewal.new_expiry_date,
                PremiumAmount = renewal.premium_amount,
                Status = renewal.Renewal_Status.ToString()
            };
        }
    }
}