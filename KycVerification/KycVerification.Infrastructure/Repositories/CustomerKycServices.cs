using KycVerification.Application.Interface;
using KycVerification.Domain.Entities;
using KycVerification.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace KycVerification.Infrastructure.Repositories
{
    public class CustomerKycServices : ICustomerKycServices
    {
        private readonly AppDbContext db;

        public CustomerKycServices(AppDbContext db)
        {
            this.db = db;
        }

        public async Task<CustomerKyc?> GetByIdAsync(int kycId)
        {
            return await db.CustomerKycs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.kyc_id == kycId);
        }

        public async Task<CustomerKyc?> GetByCustomerIdAsync(int customerId)
        {
            return await db.CustomerKycs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.customer_id == customerId);
        }

        public async Task<List<CustomerKyc>> GetAllAsync()
        {
            return await db.CustomerKycs
                .AsNoTracking()
                .OrderByDescending(x => x.kyc_date)
                .ToListAsync();
        }

        public async Task<CustomerKyc> AddAsync(CustomerKyc kyc)
        {
            kyc.kyc_id = 0;
            kyc.kyc_date = DateTime.UtcNow;
            kyc.verification_status = "Pending";
            kyc.verified_by = null;
            kyc.verified_date = null;

            await db.CustomerKycs.AddAsync(kyc);
            await db.SaveChangesAsync();

            return kyc;
        }

        public async Task UpdateAsync(CustomerKyc kyc)
        {
            var existing = await db.CustomerKycs
                .FirstOrDefaultAsync(x => x.kyc_id == kyc.kyc_id);

            if (existing == null)
            {
                throw new KeyNotFoundException(
                    $"KYC record with ID {kyc.kyc_id} was not found.");
            }

            existing.remarks = kyc.remarks;

            await db.SaveChangesAsync();
        }
    }
}