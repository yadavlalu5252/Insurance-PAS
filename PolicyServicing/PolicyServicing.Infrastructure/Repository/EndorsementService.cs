using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    public class EndorsementService : IEndorsementService
    {
        private readonly AppDbContext db;
        private readonly PolicyClient policyClient;
        private readonly PremiumClient premiumClient;
        private readonly CustomerClient customerClient;

        private readonly ClaimsClient claimsClient;

        private readonly ProductClient productClient;

        private readonly IHttpContextAccessor httpContextAccessor;

        public EndorsementService(AppDbContext db, PolicyClient policyClient, PremiumClient premiumClient, CustomerClient customerClient,
            IHttpContextAccessor httpContextAccessor,ClaimsClient claimsClient, ProductClient productClient)
        {
            this.db = db;
            this.policyClient = policyClient;
            this.premiumClient = premiumClient;
            this.customerClient = customerClient;
            this.claimsClient = claimsClient;
            this.productClient = productClient;
            this.httpContextAccessor = httpContextAccessor;

        }

        
        public async Task<policy_endorsement> CreateEndorsementAsync(CreateEndorsementDTO dto)
        {
            var request = await db.service_request.FindAsync(dto.RequestId);
            if (request == null) throw new Exception("Parent Service Request not found.");

            var endorsement = new policy_endorsement
            {
                request_id = dto.RequestId,
                endorsement_number = $"END-{DateTime.UtcNow.Ticks}",
                endorsement_type = dto.EndorsementType,
                previous_value = dto.PreviousValue,
                updated_value = dto.UpdatedValue,
                approval_status = Approval_status.Pending 
            };

            db.policy_endorsements.Add(endorsement);
            await db.SaveChangesAsync();
            return endorsement;
        }

       
        public async Task<bool> ApproveEndorsementAsync(long endorsementId, long adminId)
        {
            var token = httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString();
            var endorsement = await db.policy_endorsements
                .Include(e => e.service_Request)
                .FirstOrDefaultAsync(e => e.endorsement_id == endorsementId);

            if (endorsement == null) return false;
            if (endorsement.approval_status != Approval_status.Pending)
                throw new Exception("This endorsement is already processed.");

            long policyId = endorsement.service_Request.policy_id;
            long customerId = endorsement.service_Request.customer_id;

            try
            {
               
                switch (endorsement.endorsement_type)
                {
                    case "Address Change":
                        var newAddress = JsonSerializer.Deserialize<CustomerAddressDTO>(endorsement.updated_value);

                      
                        var allAddresses = await customerClient.GetCustomerAddresses(customerId, token);
                        var oldAddress = allAddresses.FirstOrDefault(a => a.address_type == newAddress.address_type);


                        if (oldAddress == null)
                        {
                            throw new Exception("Existing address not found in Customer Microservice.");
                        }

                        
                        endorsement.previous_value = JsonSerializer.Serialize(oldAddress);

                      
                        bool isAddressUpdated = await customerClient.UpdateAddress(oldAddress.address_id, newAddress, token);
                        if (!isAddressUpdated)
                        {
                            throw new Exception("Customer Microservice rejected the address update.");
                        }

                        break;
                    case "Contact Update":
                 
                        var contactJson = JsonDocument.Parse(endorsement.updated_value).RootElement;
                        string newEmail = contactJson.GetProperty("email").GetString();
                        string newMobile = contactJson.GetProperty("mobile_number").GetString();

                        var oldCustomer = await customerClient.GetCustomerById(customerId, token);
                        if (oldCustomer == null) throw new Exception("Customer not found in Customer Microservice.");

                
                        endorsement.previous_value = JsonSerializer.Serialize(oldCustomer);

                     
                        var existingEmail = await customerClient.SearchCustomersByEmail(newEmail, token);
                        if (existingEmail.Any(c => c.customer_id != customerId))
                            throw new Exception("Email address is already in use by another customer.");

                        var existingMobile = await customerClient.SearchCustomersByMobile(newMobile, token);
                        if (existingMobile.Any(c => c.customer_id != customerId))
                            throw new Exception("Mobile number is already in use by another customer.");

          
               
                        var updatePayload = new
                        {
                            first_name = oldCustomer.first_name,
                            middle_name = oldCustomer.middle_name,
                            last_name = oldCustomer.last_name,
                            date_of_birth = oldCustomer.date_of_birth,

                            gender = oldCustomer.gender == "0" ? "Male" :
                                     oldCustomer.gender == "1" ? "Female" : "Other",

                            marital_status = oldCustomer.marital_status == "0" ? "Single" :
                                             oldCustomer.marital_status == "1" ? "Married" :
                                             oldCustomer.marital_status == "2" ? "Divorced" : "Widowed",

                            occupation = oldCustomer.occupation,
                            annual_income = oldCustomer.annual_income,
                            mobile_number = newMobile, // The new mobile
                            email = newEmail,          // The new email

                            customer_status = oldCustomer.customer_status == "0" ? "Active" : "Inactive"
                        };

            
                        bool isContactUpdated = await customerClient.UpdateCustomer(customerId, updatePayload, token);

                        if (!isContactUpdated)
                            throw new Exception("Customer Microservice rejected the contact update.");
                        break;

                    case "Nominee Change":
                        var updatedNominees = JsonSerializer.Deserialize<List<NomineeDTO>>(endorsement.updated_value);
                        if (updatedNominees != null)
                        {

                            var oldNominees = await customerClient.GetNominees(customerId,token);

                    
                            endorsement.previous_value = JsonSerializer.Serialize(oldNominees);

           
                            foreach (var oldNom in oldNominees)
                            {
                                await customerClient.DeleteNominee(oldNom.nominee_id,token);
                            }

                            foreach (var newNom in updatedNominees)
                            {
                                bool isAdded = await customerClient.AddNominee(customerId, newNom, token);
                                if (!isAdded) throw new Exception("Customer API rejected the nominee add request.");
                            }
                        }
                        break;
                    case "Coverage Update":
                    case "Coverage Increase":
                    case "Coverage Reduction":
                        var coverageData = JsonSerializer.Deserialize<SavedCoverageDTO>(
                            endorsement.updated_value,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                        );

                        var currentCoverages = await policyClient.GetPolicyCoverages(policyId, token);
                        var oldCoverage = currentCoverages.FirstOrDefault(c => c.coverage_id == coverageData.coverage_id);

                        if (oldCoverage == null) throw new Exception("Coverage not found on policy.");


                        endorsement.previous_value = JsonSerializer.Serialize(oldCoverage);

                    
                        var coverageRules = await productClient.GetCoverageRules(coverageData.coverage_id, token);

                        if (coverageRules == null)
                            throw new Exception("Coverage rules not found in Product Microservice.");

                        if (coverageData.sum_insured < coverageRules.minimum_coverage ||
                            coverageData.sum_insured > coverageRules.maximum_coverage)
                        {
                            throw new Exception($"Sum insured must be between {coverageRules.minimum_coverage} and {coverageRules.maximum_coverage} for this coverage.");
                        }
               
   
                        var policyDetailsForPremium = await policyClient.GetPolicyById(policyId, token);

                        decimal oldSumInsured = oldCoverage.sum_insured;
                        decimal newSumInsured = coverageData.sum_insured;
                        decimal oldTotalPremium = policyDetailsForPremium.premium_amount;

                        decimal newCalculatedPremium = oldTotalPremium;
                        if (oldSumInsured > 0)
                        {
                            newCalculatedPremium = oldTotalPremium * (newSumInsured / oldSumInsured);
                        }

                    
                        oldCoverage.sum_insured = coverageData.sum_insured;

                        bool isCoverageUpdated = await policyClient.UpdatePolicyCoverage(policyId, oldCoverage.coverage_id, oldCoverage, token);
                        if (!isCoverageUpdated) throw new Exception("Core Policy coverage update failed.");

                        endorsement.updated_value = JsonSerializer.Serialize(oldCoverage);

             
                        var endorsementPayload = new
                        {
                            policy_id = policyId,
                            endorsement_type = "Coverage Update",
                            old_values = endorsement.previous_value,
                            new_values = endorsement.updated_value,
                            status = "Approved",
                            approved_by = adminId.ToString(),
                            created_date = DateTime.UtcNow,
                            approved_date = DateTime.UtcNow
                        };
                        await policyClient.CreatePolicyEndorsement(policyId, endorsementPayload, token);

       
                        var premiumPayload = new
                        {
                            policy_id = (int)policyId,
                            issue_date = policyDetailsForPremium.issue_date,
                            effective_date = policyDetailsForPremium.effective_date,
                            expiry_date = policyDetailsForPremium.expiry_date,
                            premium_frequency = "Monthly",
                            total_premium = newCalculatedPremium
                        };

                        bool isPremiumUpdated = await premiumClient.GeneratePremiumSchedule(premiumPayload, token);
                        if (!isPremiumUpdated) throw new Exception("Failed to recalculate premium.");
                        break;

                    case "Rider Addition":
                    case "Rider Removal":
                        // 1. Deserialize the requested intent (Action + RiderId)
                        var riderRequest = JsonSerializer.Deserialize<RiderModifyDTO>(
                                            endorsement.updated_value,
                                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                                        );
                        if (riderRequest == null) throw new Exception("Invalid rider payload.");

                        // 2. Fetch Current Policy Details
                        var riderPolicyDetails = await policyClient.GetPolicyById(policyId, token);
                        if (riderPolicyDetails == null) throw new Exception("Policy not found.");
                        if (riderPolicyDetails.policy_status != "Active" && riderPolicyDetails.policy_status != "3")
                        {
                            throw new Exception($"Policy must be Active to modify riders. Current status: {riderPolicyDetails.policy_status}");
                        }


                        var eligibleRiders = await productClient.GetEligibleRiders(riderPolicyDetails.product_id, token);
                        var requestedRiderInfo = eligibleRiders.FirstOrDefault(r => r.rider_id == riderRequest.RiderId);
                        if (requestedRiderInfo == null) throw new Exception("This rider is not eligible for your policy's product.");


                        var currentRiders = await policyClient.GetPolicyRiders(policyId, token);
                        var oldRidersList = currentRiders ?? new List<PolicyRiderDTO>();
                        var updatedRidersList = new List<PolicyRiderDTO>(oldRidersList);

                        // Assuming ProductRiderDTO has 'rider_premium' and 'rider_name'. Change these if your DTO uses different names.
                        decimal riderCost = requestedRiderInfo.rider_premium;
                        decimal newRiderCalculatedPremium = riderPolicyDetails.premium_amount;

                        if (riderRequest.Action == "Add")
                        {
                            if (updatedRidersList.Any(r => r.rider_id == riderRequest.RiderId))
                                throw new Exception("Policy already has this rider.");

                            updatedRidersList.Add(new PolicyRiderDTO
                            {
                                rider_id = riderRequest.RiderId,
                                policy_id = policyId,
                                rider_name = requestedRiderInfo.rider_name,
                                premium_amount = requestedRiderInfo.rider_premium
                            });
                            newRiderCalculatedPremium += riderCost;
                        }
                        else if (riderRequest.Action == "Remove")
                        {
                            var riderToRemove = updatedRidersList.FirstOrDefault(r => r.rider_id == riderRequest.RiderId);
                            if (riderToRemove == null) throw new Exception("Rider not found on this policy.");

                            updatedRidersList.Remove(riderToRemove);
                            newRiderCalculatedPremium -= riderCost;
                        }

                        endorsement.previous_value = JsonSerializer.Serialize(oldRidersList);
                        endorsement.updated_value = JsonSerializer.Serialize(updatedRidersList);

                        bool isRiderUpdated = await policyClient.UpdatePolicyRiders(policyId, updatedRidersList, token);
                        if (!isRiderUpdated) throw new Exception("Core Policy rider update failed.");

               
                        await policyClient.CreatePolicyEndorsement(policyId, new
                        {
                            policy_id = policyId,
                            endorsement_type = endorsement.endorsement_type,
                            old_values = endorsement.previous_value,
                            new_values = endorsement.updated_value,
                            status = "Approved",
                            approved_by = adminId.ToString(),
                            created_date = DateTime.UtcNow,
                            approved_date = DateTime.UtcNow
                        }, token);

                        var riderPremiumPayload = new
                        {
                            policy_id = (int)policyId,
                            issue_date = riderPolicyDetails.issue_date,
                            effective_date = riderPolicyDetails.effective_date,
                            expiry_date = riderPolicyDetails.expiry_date,
                            premium_frequency = "Monthly",
                            total_premium = newRiderCalculatedPremium
                        };

                        bool isRiderPremiumUpdated = await premiumClient.GeneratePremiumSchedule(riderPremiumPayload, token);
                        if (!isRiderPremiumUpdated) throw new Exception("Failed to recalculate premium.");
                        break;
                    default:
                        throw new Exception($"No approval logic implemented for type: {endorsement.endorsement_type}");
                }

              
                endorsement.approval_status = Approval_status.Approved;
                endorsement.service_Request.status = RequestStatus.Completed;

                db.service_request_history.Add(new service_request_history
                {
                    request_id = endorsement.request_id,
                    action = $"Endorsement Approved ({endorsement.endorsement_type})",
                    action_by = adminId,
                    action_date = DateTime.UtcNow,
                    remarks = $"{endorsement.endorsement_type} approved. Microservices updated."
                });

                await db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                
                endorsement.approval_status = Approval_status.Rejected;
                endorsement.service_Request.status = RequestStatus.Rejected;

                db.service_request_history.Add(new service_request_history
                {
                    request_id = endorsement.request_id,
                    action = $"Endorsement Approval Failed ({endorsement.endorsement_type})",
                    action_by = adminId,
                    action_date = DateTime.UtcNow,
                    remarks = ex.Message
                });

                await db.SaveChangesAsync();
                throw new Exception($"Approval Failed: {ex.Message}");
            }
        }

        public async Task<bool> RejectEndorsementAsync(long endorsementId, long adminId, string reason)
        {
            var endorsement = await db.policy_endorsements
                .Include(e => e.service_Request)
                .FirstOrDefaultAsync(e => e.endorsement_id == endorsementId);

            if (endorsement == null) return false;

            endorsement.approval_status = Approval_status.Rejected;
            endorsement.service_Request.status = RequestStatus.Rejected;

            db.service_request_history.Add(new service_request_history
            {
                request_id = endorsement.request_id,
                action = $"Endorsement Rejected ({endorsement.endorsement_type})",
                action_by = adminId,
                action_date = DateTime.UtcNow,
                remarks = reason
            });

            await db.SaveChangesAsync();
            return true;
        }


        public async Task<policy_endorsement?> GetEndorsementByIdAsync(long endorsementId)
        {
            return await db.policy_endorsements
                .Include(e => e.service_Request)
                .FirstOrDefaultAsync(e => e.endorsement_id == endorsementId);
        }

        public async Task<List<policy_endorsement>> GetAllPendingEndorsementsAsync()
        {
            return await db.policy_endorsements
                .Include(e => e.service_Request) 
                .Where(e => e.approval_status == Approval_status.Pending)
                .OrderByDescending(e => e.endorsement_id)
                .ToListAsync();
        }
    }
}