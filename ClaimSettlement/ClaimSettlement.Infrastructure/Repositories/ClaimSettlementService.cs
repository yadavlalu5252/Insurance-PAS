using ClaimSettlement.Application.Interfaces;
using ClaimSettlement.Domain.Entities;
using ClaimSettlement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;

namespace ClaimSettlement.Infrastructure.Repositories
{
    public class ClaimSettlementService : IClaimSettlementService
    {
        private readonly HttpClient http;
        private readonly AppDbContext db;
        public ClaimSettlementService(AppDbContext db, HttpClient http)
        {
            this.db = db;
            this.http = http;
        }

        public async Task<Settlement> GetByClaimId(long claimId)
        {
            var data = await db.ClaimSettlements.FirstOrDefaultAsync(x => x.ClaimId == claimId);
            return data;
        }

        public async Task Add(Settlement s)
        {
            var response = await http.GetAsync(
                "http://localhost:5129/api/v1/claims/" + s.ClaimId + "/status");

            if (!response.IsSuccessStatusCode)
                throw new Exception("Unable to get claim status");

            var claim = await response.Content.ReadFromJsonAsync<ClaimStatusDTO>();

            if (claim == null || claim.claim_status != "Approved")
                throw new Exception("Claim is not approved");

            db.ClaimSettlements.Add(s);
            await db.SaveChangesAsync();
        }


        public async Task Update(Settlement s)
        {
            db.ClaimSettlements.Update(s);
            await db.SaveChangesAsync();
        }
    }
}
