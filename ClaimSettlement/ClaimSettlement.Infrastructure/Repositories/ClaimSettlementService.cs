using ClaimSettlement.Application.Interfaces;
using ClaimSettlement.Domain.Entities;
using ClaimSettlement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ClaimSettlement.Infrastructure.Repositories
{
    public class ClaimSettlementService : IClaimSettlementService
    {
        private readonly AppDbContext db;

        public ClaimSettlementService(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<Settlement?> GetByClaimId(long claimId)
        {
            var data = await db.ClaimSettlements
                .FirstOrDefaultAsync(x => x.ClaimId == claimId);

            return data;
        }

        //public async Task Add(Settlement s)
        //{
        //    var response = await http.GetAsync(
        //        "http://localhost:5129/api/v1/claims/" + s.ClaimId + "/status");

        //    if (!response.IsSuccessStatusCode)
        //        throw new Exception("Unable to get claim status");

        //    var claim = await response.Content.ReadFromJsonAsync<ClaimStatusDTO>();

        //    if (claim == null || claim.claim_status != "Approved")
        //        throw new Exception("Claim is not approved");

        //    db.ClaimSettlements.Add(s);
        //    await db.SaveChangesAsync();
        //}

        public async Task Update(Settlement s)
        {
            if (s.ApprovedAmount <= 0)
                throw new Exception("Approved amount must be greater than zero");

            if (string.IsNullOrWhiteSpace(s.PaymentMode))
                throw new Exception("Payment mode is required");

            if (s.PaymentMode != "NEFT" &&
                s.PaymentMode != "RTGS" &&
                s.PaymentMode != "IMPS" &&
                s.PaymentMode != "UPI" &&
                s.PaymentMode != "Cheque")
            {
                throw new Exception("Invalid payment mode");
            }

            if (s.SettlementStatus != "Pending" &&
                s.SettlementStatus != "Completed" &&
                s.SettlementStatus != "Failed")
            {
                throw new Exception("Invalid settlement status");
            }

            db.ClaimSettlements.Update(s);
            await db.SaveChangesAsync();
        }
    }
}