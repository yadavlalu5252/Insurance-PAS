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

        public EndorsementService(AppDbContext db, PolicyClient policyClient, PremiumClient premiumClient, CustomerClient customerClient)
        {
            this.db = db;
            this.policyClient = policyClient;
            this.premiumClient = premiumClient;
            this.customerClient = customerClient;
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
                        var allAddresses = await customerClient.GetCustomerAddresses(customerId);
                        var oldAddress = allAddresses.FirstOrDefault(a => a.address_type == newAddress.address_type);
                        if (oldAddress != null)
                        {
                            bool isAddressUpdated = await customerClient.UpdateAddress(oldAddress.address_id, newAddress);
                            if (!isAddressUpdated) throw new Exception("Customer Microservice rejected the address update.");
                        }
                        break;

                    case "Contact Update":
                        var contactData = JsonSerializer.Deserialize<ContactUpdateDTO>(endorsement.updated_value);
                        var newCustomerData = new { mobile_number = contactData.MobileNumber, email = contactData.EmailAddress };
                        bool isContactUpdated = await customerClient.UpdateCustomer(customerId, newCustomerData);
                        if (!isContactUpdated) throw new Exception("Customer Microservice rejected the contact update.");
                        break;

                    case "Nominee Change":
                        var updatedNominees = JsonSerializer.Deserialize<List<NomineeDTO>>(endorsement.updated_value);
                        if (updatedNominees != null)
                        {
                            var oldNominees = await customerClient.GetNominees(customerId);
                            foreach (var oldNom in oldNominees) await customerClient.DeleteNominee(oldNom.nominee_id);
                            foreach (var newNom in updatedNominees) await customerClient.AddNominee(customerId, newNom);
                        }
                        break;

                    case "Coverage Increase":
                    case "Coverage Reduction":
                        var coverageData = JsonSerializer.Deserialize<SavedCoverageDTO>(endorsement.updated_value);
                        var policyPayload = new { coverage_id = coverageData.coverage_id, sum_insured = coverageData.sum_insured };

                        bool isPolicyUpdated = await policyClient.UpdatePolicy(policyId, policyPayload);
                        if (!isPolicyUpdated) throw new Exception("Core Policy update failed.");

                        await policyClient.CreatePolicyEndorsement(policyId, new { endorsement_type = endorsement.endorsement_type, new_value = endorsement.updated_value });

                        bool isPremiumUpdated = await premiumClient.GeneratePremiumSchedule(policyId);
                        if (!isPremiumUpdated) throw new Exception("Failed to recalculate premium.");
                        break;

                    case "Rider Addition":
                    case "Rider Removal":
                        var updatedRidersList = JsonSerializer.Deserialize<List<PolicyRiderDTO>>(endorsement.updated_value);
                        var riderPayload = new { riders = updatedRidersList };

                        bool isRiderUpdated = await policyClient.UpdatePolicy(policyId, riderPayload);
                        if (!isRiderUpdated) throw new Exception("Core Policy update failed.");

                        await policyClient.CreatePolicyEndorsement(policyId, new { endorsement_type = endorsement.endorsement_type, new_value = endorsement.updated_value });

                        bool isRiderPremiumUpdated = await premiumClient.GeneratePremiumSchedule(policyId);
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
    }
}