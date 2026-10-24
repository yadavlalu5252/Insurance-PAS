
using Microsoft.EntityFrameworkCore;
using PolicyIssuance.Application.DTOs;
using PolicyIssuance.Application.Interface;
using PolicyIssuance.Domain.Entities;
using PolicyIssuance.Domain.Enums;
using PolicyIssuance.Infrastructure.Clients;
using PolicyIssuance.Infrastructure.Data;

namespace PolicyIssuance.Infrastructure.Services
{
    public class PolicyService : IPolicyService
    {
        private readonly AppDbContext db;
        private readonly CustomerClient customerClient;
        private readonly KycClient kycClient;
        private readonly QuoteClient quoteClient;
        private readonly UnderwritingClient underwritingClient;
        private readonly ProductClient productClient;
        private readonly PremiumPaymentClient premiumPaymentClient;

        public PolicyService(
            AppDbContext db,
            CustomerClient customerClient,
            KycClient kycClient,
            QuoteClient quoteClient,
            UnderwritingClient underwritingClient,
            ProductClient productClient,
            PremiumPaymentClient premiumPaymentClient)
        {
            this.db = db;
            this.customerClient = customerClient;
            this.kycClient = kycClient;
            this.quoteClient = quoteClient;
            this.underwritingClient = underwritingClient;
            this.productClient = productClient;
            this.premiumPaymentClient = premiumPaymentClient;
        }

        public async Task<List<PolicyResponseDTO>> GetAllAsync()
        {
            var policies = await db.Policies
                .AsNoTracking()
                .OrderByDescending(p => p.issue_date)
                .ToListAsync();

            return policies.Select(ToResponse).ToList();
        }

        public async Task<PolicyResponseDTO?> GetByIdAsync(long id)
        {
            var policy = await db.Policies
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.policy_id == id);

            if (policy == null)
                return null;

            return ToResponse(policy);
        }

        public async Task<PolicyResponseDTO?> CreateAsync(PolicyDTO dto)
        {
            if (dto.customer_id <= 0 ||
                dto.quote_id <= 0 ||
                dto.product_id <= 0 ||
                dto.underwriting_id <= 0 ||
                dto.premium_amount <= 0 ||
                dto.effective_date >= dto.expiry_date)
            {
                return null;
            }

            var customer = await customerClient.FindCustomerById(
                dto.customer_id);

            if (customer == null)
                return null;

            var kyc = await kycClient.FindKycByCustomerId(
                dto.customer_id);

            if (kyc == null ||
                !string.Equals(
                    kyc.verification_status,
                    "Verified",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var quote = await quoteClient.FindQuoteById(dto.quote_id);

            if (quote == null ||
                quote.customer_id != dto.customer_id ||
                quote.product_id != dto.product_id)
            {
                return null;
            }

            var underwriting =
                await underwritingClient.FindUnderwritingById(
                    dto.underwriting_id);

            if (underwriting == null ||
                underwriting.quote_id != dto.quote_id ||
                underwriting.customer_id != dto.customer_id ||
                underwriting.product_id != dto.product_id)
            {
                return null;
            }

            var product = await productClient.FindProductById(
                dto.product_id);

            if (product == null ||
                !string.Equals(
                    product.status,
                    "Active",
                    StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            bool quoteExists = await db.Policies
                .AnyAsync(p => p.quote_id == dto.quote_id);

            if (quoteExists)
                return null;

            var policy = new PolicyEntity
            {
                policy_number = "POL-" +
                    Guid.NewGuid().ToString("N").ToUpperInvariant(),

                customer_id = dto.customer_id,
                quote_id = dto.quote_id,
                product_id = dto.product_id,
                underwriting_id = dto.underwriting_id,
                issue_date = dto.issue_date == default
                    ? DateTime.UtcNow
                    : dto.issue_date,
                effective_date = dto.effective_date,
                expiry_date = dto.expiry_date,
                premium_amount = dto.premium_amount,
                policy_status = PolicyStatus.PendingPayment
            };

            db.Policies.Add(policy);
            await db.SaveChangesAsync();

            return ToResponse(policy);
        }

        public async Task<PolicyResponseDTO?> UpdateAsync(
            long id, PolicyDTO dto)
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.policy_id == id);

            if (policy == null ||
                policy.policy_status != PolicyStatus.Draft)
            {
                return null;
            }

            if (dto.customer_id <= 0 ||
                dto.quote_id <= 0 ||
                dto.product_id <= 0 ||
                dto.underwriting_id <= 0 ||
                dto.premium_amount <= 0 ||
                dto.effective_date >= dto.expiry_date)
            {
                return null;
            }

            bool duplicateQuote = await db.Policies.AnyAsync(
                p => p.quote_id == dto.quote_id &&
                     p.policy_id != id);

            if (duplicateQuote)
                return null;

            policy.customer_id = dto.customer_id;
            policy.quote_id = dto.quote_id;
            policy.product_id = dto.product_id;
            policy.underwriting_id = dto.underwriting_id;
            policy.effective_date = dto.effective_date;
            policy.expiry_date = dto.expiry_date;
            policy.premium_amount = dto.premium_amount;

            await db.SaveChangesAsync();

            return ToResponse(policy);
        }

        public async Task<bool> DeleteAsync(long id)
        {
            return await CancelAsync(id, "system");
        }

        public async Task<bool> ActivateAsync(
            long id, string performed_by)
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.policy_id == id);

            if (policy == null ||
                policy.policy_status != PolicyStatus.PendingPayment)
            {
                return false;
            }

            bool paymentHistoryAvailable =
                await premiumPaymentClient.CheckPaymentHistory(
                    policy.policy_id);

            if (!paymentHistoryAvailable)
                return false;

            policy.policy_status = PolicyStatus.Active;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SuspendAsync(
            long id, string performed_by)
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.policy_id == id);

            if (policy == null ||
                policy.policy_status != PolicyStatus.Active)
            {
                return false;
            }

            policy.policy_status = PolicyStatus.Suspended;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CancelAsync(
            long id, string performed_by)
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.policy_id == id);

            if (policy == null ||
                policy.policy_status == PolicyStatus.Cancelled ||
                policy.policy_status == PolicyStatus.Expired ||
                policy.policy_status == PolicyStatus.Closed)
            {
                return false;
            }

            policy.policy_status = PolicyStatus.Cancelled;

            await db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> ExpireAsync(
            long id, string performed_by)
        {
            var policy = await db.Policies
                .FirstOrDefaultAsync(p => p.policy_id == id);

            if (policy == null ||
                policy.policy_status != PolicyStatus.Active ||
                policy.expiry_date > DateTime.UtcNow)
            {
                return false;
            }

            policy.policy_status = PolicyStatus.Expired;

            await db.SaveChangesAsync();

            return true;
        }


        public async Task<List<PolicyCoverageDTO>> GetCoveragesAsync(long policyId)
        {
            // Assuming she has a DbSet called PolicyCoverages in her DbContext
            return await db.PolicyCoverages
                .Where(c => c.policy_id == policyId)
                .Select(c => new PolicyCoverageDTO
                {
                    coverage_id = c.coverage_id,
                    policy_id = c.policy_id,
                    coverage_name = c.coverage_name,
                    sum_insured = c.sum_insured,
                    coverage_start_date = c.coverage_start_date,
                    coverage_end_date = c.coverage_end_date
                })
                .ToListAsync();
        }

        private static PolicyResponseDTO ToResponse(
            PolicyEntity policy)
        {
            return new PolicyResponseDTO
            {
                policy_id = policy.policy_id,
                policy_number = policy.policy_number,
                customer_id = policy.customer_id,
                quote_id = policy.quote_id,
                product_id = policy.product_id,
                underwriting_id = policy.underwriting_id,
                issue_date = policy.issue_date,
                effective_date = policy.effective_date,
                expiry_date = policy.expiry_date,
                premium_amount = policy.premium_amount,
                policy_status = policy.policy_status.ToString(),
                customer_name = string.Empty,
                product_name = string.Empty
            };
        }


        public async Task<PolicyCoverageDTO?> UpdateCoverageAsync(long policyId, long coverageId, PolicyCoverageDTO dto)
        {
            // 1. Find the coverage in her database
            var coverage = await db.PolicyCoverages
                .FirstOrDefaultAsync(c => c.policy_id == policyId && c.coverage_id == coverageId);

            if (coverage == null)
            {
                return null; // Coverage not found
            }

            // 2. Update the allowed fields (usually just the sum insured)
            coverage.sum_insured = dto.sum_insured;
            coverage.coverage_name = dto.coverage_name;

            // 3. Save changes
            db.PolicyCoverages.Update(coverage);
            await db.SaveChangesAsync();

            // 4. Return the updated DTO
            return new PolicyCoverageDTO
            {
                coverage_id = coverage.coverage_id,
                policy_id = coverage.policy_id,
                coverage_name = coverage.coverage_name,
                sum_insured = coverage.sum_insured,
                coverage_start_date = coverage.coverage_start_date,
                coverage_end_date = coverage.coverage_end_date
            };

            // Note: If she uses AutoMapper, she can just return _mapper.Map<PolicyCoverageDTO>(coverage);
        }

        public async Task<bool> UpdateRidersAsync(long policyId, List<PolicyRiderDTO> riders)
        {
            // 1. Verify the policy exists and is Active
            var policy = await db.Policies.FirstOrDefaultAsync(p => p.policy_id == policyId);

            // 🚨 FIX: Use the Enum value instead of the string "Active"
            if (policy == null || policy.policy_status != PolicyStatus.Active)
                return false;

            // 2. Delete ALL existing riders for this policy
            var existingRiders = await db.PolicyRiders
                .Where(r => r.policy_id == policyId)
                .ToListAsync();

            if (existingRiders.Any())
            {
                db.PolicyRiders.RemoveRange(existingRiders);
            }

            // 3. Add the new list of riders
            foreach (var newRider in riders)
            {
                db.PolicyRiders.Add(new PolicyRider
                {
                    policy_id = policyId,
                    rider_id = newRider.rider_id,
                    rider_name = newRider.rider_name,
                    premium_amount = newRider.premium_amount
                });
            }

            // 4. Save changes
            await db.SaveChangesAsync();

            return true;
        }

        public async Task<List<PolicyRiderDTO>> GetRidersAsync(long policyId)
        {
            // Fetch from database and map manually using Select
            var riders = await db.PolicyRiders
                .Where(r => r.policy_id == policyId)
                .Select(r => new PolicyRiderDTO
                {
                    rider_id = r.rider_id,
                    policy_id = r.policy_id,
                    rider_name = r.rider_name,
                    premium_amount = r.premium_amount
                })
                .ToListAsync();

            return riders;
        }

        public async Task<bool> RenewPolicyAsync(long id, DateTime newExpiryDate)
        {
            // 1. Find the policy
            var policy = await db.Policies.FirstOrDefaultAsync(p => p.policy_id == id);
            if (policy == null) return false;

            // 2. Check if it's Active (using her Enum)
            if (policy.policy_status != PolicyStatus.Active)
                return false;

            // 3. Update ONLY the expiry date
            policy.expiry_date = newExpiryDate;

            // 4. Save to database
            await db.SaveChangesAsync();
            return true;
        }

        public async Task<List<PolicyDTO>> GetPoliciesByCustomerIdAsync(long customerId)
        {
            // 1. Fetch all policies from the database that belong to this customer
            var policies = await db.Policies
                .Where(p => p.customer_id == customerId)
                .ToListAsync();

            // 2. Map them to DTOs (using manual mapping to be safe)
            var result = policies.Select(p => new PolicyDTO
            {
                policy_id = p.policy_id,
                policy_number = p.policy_number,
                customer_id = p.customer_id,
                product_id = p.product_id,
                issue_date = p.issue_date,
                effective_date = p.effective_date,
                expiry_date = p.expiry_date,
                premium_amount = p.premium_amount,
                policy_status = p.policy_status.ToString() // If her status is an Enum, convert to string
            }).ToList();

            return result;
        }
    }
}
