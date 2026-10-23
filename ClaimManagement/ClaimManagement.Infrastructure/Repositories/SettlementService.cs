using ClaimManagement.Application.DTO;
using ClaimManagement.Application.Exceptions;
using ClaimManagement.Application.Interface;
using ClaimManagement.Domain.Entities;
using ClaimManagement.Domain.Rules;
using ClaimManagement.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class SettlementService : ISettlementService
    {
        ClaimDbContext db;
        ISecretProtector protector;

        // BRD: payment modes
        static readonly string[] PaymentModes = { "NEFT", "RTGS", "IMPS", "UPI", "Cheque" };

        public SettlementService(ClaimDbContext db, ISecretProtector protector)
        {
            this.db = db;
            this.protector = protector;
        }

        public async Task<SettlementResponseDTO> ProcessAsync(long claimId, ProcessSettlementDTO dto, long performedBy)
        {
            if (!PaymentModes.Contains(dto.payment_mode))
                throw new BadRequestException("Payment mode must be one of: " + string.Join(", ", PaymentModes));
            if (string.IsNullOrWhiteSpace(dto.transaction_reference) || dto.transaction_reference.Length > 100)
                throw new BadRequestException("Transaction reference is required (max 100 characters)");
            if (string.IsNullOrWhiteSpace(dto.account_holder_name) || dto.account_holder_name.Length > 150)
                throw new BadRequestException("Account holder name is required (max 150 characters)");
            if (string.IsNullOrWhiteSpace(dto.account_number) || !Regex.IsMatch(dto.account_number, @"^\d{9,18}$"))
                throw new BadRequestException("Account number must be 9 to 18 digits");
            if (string.IsNullOrWhiteSpace(dto.ifsc_code) || !Regex.IsMatch(dto.ifsc_code.ToUpperInvariant(), @"^[A-Z]{4}0[A-Z0-9]{6}$"))
                throw new BadRequestException("IFSC code is invalid");

            // BRD: bank account verification is mandatory
            if (!dto.bank_verified)
                throw new BadRequestException("Bank account must be verified before settlement");

            var claim = await db.claim_master.FirstOrDefaultAsync(c => c.claim_id == claimId);
            if (claim == null)
                throw new NotFoundException("Claim not found");

            // BRD: settlement only after approval
            if (claim.claim_status == ClaimStatuses.Settled)
                throw new ConflictException("This claim is already settled");
            if (!ClaimStatusRules.CanMove(claim.claim_status, ClaimStatuses.Settled))
                throw new ConflictException("Settlement is allowed only for an approved claim");

            var assessment = await db.claim_assessment.FirstOrDefaultAsync(a => a.claim_id == claimId);
            if (assessment == null)
                throw new ConflictException("Claim assessment is missing for this claim");

            // TODO: "settlement cannot exceed the policy's sum insured" (BRD) is NOT checked yet.
            // It needs the sum insured from the Policy service.
            // TODO: no real payment happens here. This records a payment made outside the system.

            var settlement = new ClaimSettlement
            {
                claim_id = claimId,
                approved_amount = assessment.net_payable,
                payment_mode = dto.payment_mode,
                transaction_reference = protector.Protect(dto.transaction_reference),
                settlement_date = DateTime.Now,
                settlement_status = SettlementStatuses.Paid,
                account_holder_name = dto.account_holder_name,
                account_number = protector.Protect(dto.account_number),
                ifsc_code = dto.ifsc_code.ToUpperInvariant(),
                bank_verified = true,
                processed_by = performedBy
            };
            db.claim_settlement.Add(settlement);

            // the claim becomes Settled, and both changes are saved together
            claim.claim_status = ClaimStatuses.Settled;
            db.claim_history.Add(new ClaimHistory
            {
                claim_id = claimId,
                action = "Claim Settled",
                performed_by = performedBy,
                action_date = DateTime.Now,
                // never the full reference or account number in the history
                remarks = "Amount: " + settlement.approved_amount + ", mode: " + dto.payment_mode +
                          ", reference: " + Mask(dto.transaction_reference)
            });

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException ex) when (ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627))
            {
                // the unique index on claim_id stopped a second settlement
                throw new ConflictException("A settlement already exists for this claim");
            }

            return ToResponse(settlement);
        }

        public async Task<SettlementResponseDTO> GetAsync(long claimId)
        {
            var settlement = await GetSettlementOrThrowAsync(claimId);
            return ToResponse(settlement);
        }

        public async Task<SettlementAdviceDTO> GetAdviceAsync(long claimId)
        {
            var settlement = await GetSettlementOrThrowAsync(claimId);

            var claim = await db.claim_master.FirstAsync(c => c.claim_id == claimId);
            var assessment = await db.claim_assessment.FirstAsync(a => a.claim_id == claimId);

            return new SettlementAdviceDTO
            {
                advice_number = "ADV-" + settlement.settlement_id.ToString("D6"),
                generated_on = DateTime.Now,
                claim_number = claim.claim_number,
                policy_id = claim.policy_id,
                customer_id = claim.customer_id,
                claim_type = claim.claim_type,
                assessed_amount = assessment.assessed_amount,
                deductible = assessment.deductible,
                depreciation = assessment.depreciation,
                non_payable = assessment.non_payable,
                net_payable = assessment.net_payable,
                payment_mode = settlement.payment_mode,
                transaction_reference = Mask(protector.Unprotect(settlement.transaction_reference)),
                settlement_date = settlement.settlement_date,
                settlement_status = settlement.settlement_status,
                account_holder_name = settlement.account_holder_name,
                account_number = Mask(protector.Unprotect(settlement.account_number))
            };
        }

        // ---------- helper methods ----------

        private async Task<ClaimSettlement> GetSettlementOrThrowAsync(long claimId)
        {
            bool claimExists = await db.claim_master.AnyAsync(c => c.claim_id == claimId);
            if (!claimExists)
                throw new NotFoundException("Claim not found");

            var settlement = await db.claim_settlement.FirstOrDefaultAsync(s => s.claim_id == claimId);
            if (settlement == null)
                throw new NotFoundException("No settlement found for this claim");

            return settlement;
        }

        // manual mapping, because the stored values are encrypted and must be masked
        private SettlementResponseDTO ToResponse(ClaimSettlement s)
        {
            return new SettlementResponseDTO
            {
                settlement_id = s.settlement_id,
                claim_id = s.claim_id,
                approved_amount = s.approved_amount,
                payment_mode = s.payment_mode,
                transaction_reference = Mask(protector.Unprotect(s.transaction_reference)),
                settlement_date = s.settlement_date,
                settlement_status = s.settlement_status,
                account_holder_name = s.account_holder_name,
                account_number = Mask(protector.Unprotect(s.account_number)),
                ifsc_code = s.ifsc_code,
                bank_verified = s.bank_verified
            };
        }

        // shows only the last 4 characters, for example ********3456
        private static string Mask(string value)
        {
            if (value.Length <= 4)
                return new string('*', value.Length);

            return new string('*', value.Length - 4) + value.Substring(value.Length - 4);
        }
    }
}
